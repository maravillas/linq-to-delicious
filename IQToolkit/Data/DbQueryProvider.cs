// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQ.Data 
{
    /// <summary>
    /// A LINQ IQueryable query provider that executes database queries over a DbConnection
    /// </summary>
    public class DbQueryProvider : QueryProvider
    {
        DbConnection connection;
        QueryPolicy policy;
        QueryMapping mapping;
        QueryLanguage language;
        TextWriter log;

        public DbQueryProvider(DbConnection connection, QueryPolicy policy, TextWriter log)
        {
            this.connection = connection;
            this.policy = policy;
            this.mapping = policy.Mapping;
            this.language = mapping.Language;
            this.log = log;
        }

        public DbConnection Connection
        {
            get { return this.connection; }
        }

        public TextWriter Log
        {
            get { return this.log; }
        }

        public QueryPolicy Policy
        {
            get { return this.policy; }
        }

        public QueryMapping Mapping
        {
            get { return this.mapping; }
        }

        public QueryLanguage Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Converts the query expression into text that corresponds to the command that would be executed.
        /// Useful for debugging.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override string GetQueryText(Expression expression)
        {
            Expression translated = this.Translate(expression);
            var selects = SelectGatherer.Gather(translated).Select(s => this.language.Format(s));
            return string.Join("\n\n", selects.ToArray());
        }

        public string GetQueryPlan(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);
            return DbExpressionWriter.WriteToString(plan);
        }

        /// <summary>
        /// Execute the query expression (does translation, etc.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override object Execute(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);

            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
            {
                // compile & return the execution plan so it can be used multiple times
                LambdaExpression fn = Expression.Lambda(lambda.Type, plan, lambda.Parameters);
                return fn.Compile();
            }
            else
            {
                // compile the execution plan and invoke it
                Expression<Func<object>> efn = Expression.Lambda<Func<object>>(Expression.Convert(plan, typeof(object)));
                Func<object> fn = efn.Compile();
                return fn();
            }
        }

        /// <summary>
        /// Convert the query expression into an execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual Expression GetExecutionPlan(Expression expression)
        {
            // strip off lambda for now
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
                expression = lambda.Body;

            // translate query into client & server parts
            ProjectionExpression projection = this.Translate(expression);

            Expression rootQueryable = RootQueryableFinder.Find(expression);
            Expression provider = Expression.Convert(
                Expression.Property(rootQueryable, typeof(IQueryable).GetProperty("Provider")),
                typeof(DbQueryProvider)
                );

            return this.policy.BuildExecutionPlan(projection, provider);
        }

        /// <summary>
        /// Do all query translations execpt building the execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual ProjectionExpression Translate(Expression expression)
        {
            // pre-evaluate local sub-trees
            expression = PartialEvaluator.Eval(expression, this.CanBeEvaluatedLocally);

            // apply mapping (binds LINQ operators too)
            expression = this.mapping.Translate(expression);

            // any policy specific translations or validations
            expression = this.policy.Translate(expression);

            // any language specific translations or validations
            expression = this.language.Translate(expression);

            // do final reduction
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);

            return (ProjectionExpression)expression;
        }

        /// <summary>
        /// Determines whether a given expression can be executed locally. 
        /// (It contains no parts that should be translated to the target environment.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual bool CanBeEvaluatedLocally(Expression expression)
        {
            // any operation on a query can't be done locally
            ConstantExpression cex = expression as ConstantExpression;
            if (cex != null)
            {
                IQueryable query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                    return false;
            }
            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null &&
                (mc.Method.DeclaringType == typeof(Enumerable) ||
                 mc.Method.DeclaringType == typeof(Queryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert &&
                expression.Type == typeof(object))
                return true;
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }

        /// <summary>
        /// Execute an actual query specified in the target language using the sADO connection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Execute<T>(QueryCommand<T> query, object[] paramValues)
        {
            DbCommand cmd = this.GetCommand(query.CommandText, query.ParameterNames, paramValues);
            this.LogCommand(cmd);
            DbDataReader reader = cmd.ExecuteReader();
            return Project(reader, query.Projector);
        }

        /// <summary>
        /// Converts a data reader into a sequence of objects using a projector function on each row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fnProject"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Project<T>(DbDataReader reader, Func<DbDataReader, T> fnProjector)
        {
            while (reader.Read())
            {
                yield return fnProjector(reader);
            }
        }

        /// <summary>
        /// Get an IEnumerable that will execute the specified query when enumerated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteDeferred<T>(QueryCommand<T> query, object[] paramValues)
        {
            DbCommand cmd = this.GetCommand(query.CommandText, query.ParameterNames, paramValues);
            this.LogCommand(cmd);
            DbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return query.Projector(reader);
            }
        }

        /// <summary>
        /// Get an ADO command object initialized with the command-text and parameters
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual DbCommand GetCommand(string commandText, IList<string> paramNames, object[] paramValues)
        {
            // create command object (and fill in parameters)
            DbCommand cmd = this.connection.CreateCommand();
            cmd.CommandText = commandText;
            for (int i = 0, n = paramNames.Count; i < n; i++)
            {
                DbParameter p = cmd.CreateParameter();
                p.ParameterName = paramNames[i];
                p.Value = paramValues[i] ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
            return cmd;
        }

        /// <summary>
        /// Write a command to the log
        /// </summary>
        /// <param name="command"></param>
        protected virtual void LogCommand(DbCommand command)
        {
            if (this.log != null)
            {
                this.log.WriteLine(command.CommandText);
                foreach(DbParameter p in command.Parameters)
                {
                    if (p.Value == null || p.Value == DBNull.Value)
                    {
                        this.log.WriteLine("-- @{0} = NULL", p.ParameterName);
                    }
                    else
                    {
                        this.log.WriteLine("-- @{0} = [{1}]", p.ParameterName, p.Value);
                    }
                }
                this.log.WriteLine();
            }
        }
    }
}
