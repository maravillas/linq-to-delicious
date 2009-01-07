using System;
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

        public Delicious(WebClient webClient)
        {
            Delayer delayer = new Delayer(1000);
            QueryTranslatorFactory factory = new QueryTranslatorFactory();

            QueryProvider provider = new DeliciousQueryProvider(webClient, delayer, factory);

            Posts = new Query<Post>(provider);
        }
    }
}
