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
        private WebClient mWebClient;
        private IDelayer mDelayer;
        private IQueryTranslatorFactory mTranslatorFactory;

        public DeliciousQueryProvider(WebClient webClient, IDelayer delayer, IQueryTranslatorFactory translatorFactory)
        {
            mWebClient = webClient;
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

                Stream stream = mWebClient.OpenRead(uri);

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
            });
        }
    }
}
