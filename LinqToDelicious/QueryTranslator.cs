﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

using IQ;

namespace LinqToDelicious
{
    class QueryTranslator : ExpressionVisitor, LinqToDelicious.IQueryTranslator
    {
        public Expression Expression { get; private set; }

        private StringBuilder mBuilder;
        private Stack<Object> mStack;

        private const String TAG_TOKEN = "tags";
        private const String DATE_TOKEN = "date";

        public QueryTranslator(Expression expression)
        {
            Expression = expression;

            mStack = new Stack<Object>();
        }

        public String Translate()
        {
            if (mBuilder == null)
            {
                mBuilder = new StringBuilder("http://www.example.com/delicious.xml?");

                Visit(Expression);
            }

            return mBuilder.ToString();
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            Debug.WriteLine("Visiting method " + methodCall);

            if (methodCall.Method.DeclaringType == typeof(Queryable) && methodCall.Method.Name == "Where")
            {
                //mBuilder.Append(string.Format("Where {0}, {1}", methodCall.Arguments[0], methodCall.Arguments[1]));
                Debug.WriteLine("Type: " + ((ConstantExpression)methodCall.Arguments[0]).Value);

                LambdaExpression lambda = (LambdaExpression)StripQuotes(methodCall.Arguments[1]);

                Visit(lambda.Body);

                return methodCall;
            }
            else if (methodCall.Method.Name == "Contains")
            {
                Visit(methodCall.Object);

                String token = (String)mStack.Pop();
                
                if (token.Equals(TAG_TOKEN) &&
                    methodCall.Method.DeclaringType == typeof(List<string>))
                {
                    // Would it be reasonable to assume these conditions are true?
                    if (methodCall.Arguments.Count == 1 &&
                        methodCall.Arguments[0].NodeType == ExpressionType.Constant)
                    {
                        mBuilder.Append("&tag=");

                        Visit(methodCall.Arguments[0]);

                        mBuilder.Append(mStack.Pop());
                    }
                    else
                    {
                        throw new Exception("Missing or invalid argument to method Contains");
                    }
                }

                return methodCall;
            }
            
            // Where Query(LinqToDelicious.Post), post => (post.Date > new DateTime(2008, 1, 1))
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", methodCall.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u) 
        {
            Debug.WriteLine("Visiting unary expression " + u);

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            Debug.WriteLine("Visiting binary expression " + binaryExpression);

            if (binaryExpression.NodeType == ExpressionType.And)
            {
                Visit(binaryExpression.Left);
                Visit(binaryExpression.Right);
            }
            else if (binaryExpression.NodeType == ExpressionType.Equal ||
                binaryExpression.NodeType == ExpressionType.LessThan ||
                binaryExpression.NodeType == ExpressionType.LessThanOrEqual ||
                binaryExpression.NodeType == ExpressionType.GreaterThan ||
                binaryExpression.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                Visit(binaryExpression.Left);

                Debug.Assert(mStack.Peek().GetType() == typeof(String), "Expected String on the stack, was " + mStack.Peek().GetType());

                String token = (String)mStack.Pop();

                if (token.Equals(DATE_TOKEN))
                {
                    Visit(binaryExpression.Right);

                    Debug.Assert(mStack.Peek().GetType() == typeof(DateTime), "Expected DateTime on the stack, was " + mStack.Peek().GetType());

                    DateTime date = (DateTime)mStack.Pop();

                    switch (binaryExpression.NodeType)
                    {
                        case ExpressionType.Equal:
                            mBuilder.Append(String.Format("&fromdt={0}Z&todt={1}Z", date.ToString("s"), date.AddDays(1).ToString("s")));

                            break;

                        case ExpressionType.LessThan:
                            mBuilder.Append(String.Format("&todt={0}Z", date.ToString("s")));

                            break;

                        case ExpressionType.LessThanOrEqual:
                            date = date.AddDays(1);

                            mBuilder.Append(String.Format("&todt={0}Z", date.ToString("s")));

                            break;

                        case ExpressionType.GreaterThan:
                            mBuilder.Append(String.Format("&fromdt={0}Z", date.ToString("s")));

                            break;

                        case ExpressionType.GreaterThanOrEqual:
                            date = date.AddDays(-1);

                            mBuilder.Append(String.Format("&fromdt={0}Z", date.ToString("s")));

                            break;

                        default:
                            throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported for date comparisons", binaryExpression.NodeType));
                    }
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("The operator '{0}' is not supported", binaryExpression.NodeType));
            }


            return binaryExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            Debug.WriteLine("Visiting constant " + constant);

            mStack.Push(constant.Value);

            return constant;
        }

        protected override Expression VisitMemberAccess(MemberExpression member)
        {
            Debug.WriteLine("Visiting member " + member);

            if (member.Expression != null && 
                member.Expression.NodeType == ExpressionType.Parameter)
            {
                mStack.Push(member.Member.Name.ToLower());

                return member;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", member.Member.Name));
        }
    }
}
