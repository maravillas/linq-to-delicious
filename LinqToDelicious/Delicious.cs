﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using IQ;

namespace LinqToDelicious
{
    public class Delicious
    {
        public Query<Post> Posts { get; private set; }

        public Delicious()
        {
            Delayer delayer = new Delayer(1000);
            QueryTranslatorFactory translatorFactory = new QueryTranslatorFactory();
            IHttpWebRequestFactory requestFactory = new HttpWebRequestFactory();

            QueryProvider provider = new DeliciousQueryProvider(requestFactory, delayer, translatorFactory);

            Posts = new Query<Post>(provider);
        }
    }
}
