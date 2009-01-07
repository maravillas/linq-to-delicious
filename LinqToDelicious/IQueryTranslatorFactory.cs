using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqToDelicious
{
    interface IQueryTranslatorFactory
    {
        IQueryTranslator Create(Expression expression);
    }
}
