using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;

namespace LinqToDelicious
{
    /// <summary>
    /// Creates HttpWebRequest objects from URIs.
    /// </summary>
    class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        private string mUsername;
        private string mPassword;

        public HttpWebRequestFactory(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }
            
        /// <summary>
        /// Create a new HttpWebRequest.
        /// </summary>
        /// <param name="uri">The URI To request.</param>
        /// <returns>The request.</returns>
        public HttpWebRequest Create(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            AssemblyName name = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            request.UserAgent = name.Name + " " + name.Version.ToString();
            request.Credentials = new NetworkCredential(mUsername, mPassword);

            return request;
        }
    }
}
