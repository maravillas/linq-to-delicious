using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToDelicious
{
    class RequestException : ApplicationException
    {
        public RequestException() { }
        public RequestException(string message) : base(message) { }
        public RequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
