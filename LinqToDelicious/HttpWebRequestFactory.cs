using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LinqToDelicious
{
    /// <summary>
    /// Creates HttpWebRequest objects from URIs.
    /// </summary>
    class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        /// <summary>
        /// Create a new HttpWebRequest.
        /// </summary>
        /// <param name="uri">The URI To request.</param>
        /// <returns>The request.</returns>
        public HttpWebRequest Create(string uri)
        {
            return (HttpWebRequest)WebRequest.Create(uri);
        }
    }
}
