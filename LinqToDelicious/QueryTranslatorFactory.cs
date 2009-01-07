using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqToDelicious
{
    class QueryTranslatorFactory : IQueryTranslatorFactory
    {
        public IQueryTranslator Create(Expression expression)
        {
            return new QueryTranslator(expression);
        }
    }
}
