using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqToDelicious
{
    /// <summary>
    /// A factory for creating IQueryTranslator objects.
    /// </summary>
    interface IQueryTranslatorFactory
    {
        /// <summary>
        /// Create a new IQueryTranslator.
        /// </summary>
        /// <param name="expression">The expression to be translated.</param>
        /// <returns>The translator.</returns>
        IQueryTranslator Create(Expression expression);
    }
}
