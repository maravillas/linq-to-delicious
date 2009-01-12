using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToDelicious
{
    /// <summary>
    /// The exception that is thrown when a problem occurs while trying to request a Delicious page.
    /// </summary>
    class RequestException : ApplicationException
    {
        public RequestException() { }
        public RequestException(string message) : base(message) { }
        public RequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
