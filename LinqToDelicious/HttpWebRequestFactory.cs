using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LinqToDelicious
{
    class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        public HttpWebRequest CreateRequest(string uri)
        {
            return (HttpWebRequest)WebRequest.Create(uri);
        }
    }
}
