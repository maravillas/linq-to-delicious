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
    interface IHttpWebRequestFactory
    {
        /// <summary>
        /// Create a new HttpWebRequest.
        /// </summary>
        /// <param name="uri">The URI To request.</param>
        /// <returns>The request.</returns>
        HttpWebRequest Create(string uri);
    }
}
