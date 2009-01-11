using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LinqToDelicious
{
    interface IHttpWebRequestFactory
    {
        HttpWebRequest CreateRequest(string uri);
    }
}
