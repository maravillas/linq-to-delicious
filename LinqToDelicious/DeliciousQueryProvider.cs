using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using IQ;
using System.Diagnostics;

namespace LinqToDelicious
{
    class DeliciousQueryProvider : QueryProvider
    {
        private const int BACKOFF_DELAY = 10000;

        private IHttpWebRequestFactory mRequestFactory;
        private IDelayer mDelayer;
        private IQueryTranslatorFactory mTranslatorFactory;

        public DeliciousQueryProvider(IHttpWebRequestFactory requestFactory, IDelayer delayer, IQueryTranslatorFactory translatorFactory)
        {
            mRequestFactory = requestFactory;
            mDelayer = delayer;
            mTranslatorFactory = translatorFactory;
        }

        public override string GetQueryText(Expression expression)
        {
            return mTranslatorFactory.Create(expression).Translate();
        }

        public override object Execute(Expression expression)
        {
            return mDelayer.Delay(delegate()
            {
                expression = PartialEvaluator.Eval(expression);

                String uri = GetQueryText(expression);

                Debug.WriteLine("Requesting " + uri);

                HttpWebRequest request = mRequestFactory.Create(uri);

                HttpWebResponse response;

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    throw new RequestException("The request to " + uri + "  timed out", ex);
                }

                try
                {
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        // Simple backoff, for now.
                        mDelayer.AdditionalDelay = BACKOFF_DELAY;

                        throw new RequestException("Could not read " + uri);
                    }
                    // Is this too strict?
                    else if (response.StatusCode != HttpStatusCode.OK)
                    {
                        // TODO: Define an exception
                        throw new RequestException("Could not read " + uri);
                    }
                    else
                    {
                        Stream stream = response.GetResponseStream();

                        XDocument document = XDocument.Load(new StreamReader(stream));

                        var posts = from post in document.Descendants("post")
                                    select new Post(post.Attribute("href").Value,
                                                    post.Attribute("hash").Value,
                                                    post.Attribute("description").Value,
                                                    post.Attribute("tag").Value,
                                                    post.Attribute("extended").Value,
                                                    post.Attribute("time").Value,
                                                    post.Attribute("meta").Value);

                        return posts;
                    }
                }
                finally
                {
                    response.Close();
                }
            });
        }
    }
}
