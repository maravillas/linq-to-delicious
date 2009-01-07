using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using LinqToDelicious;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace LinqToDeliciousTest
{
    /// <summary>
    /// Summary description for DeliciousQueryProviderTest
    /// </summary>
    [TestClass]
    public class DeliciousQueryProviderTest
    {
        delegate object CallbackDelegate(Callback callback);

        private const string DOCUMENT = @"<?xml version='1.0' encoding='UTF-8'?>
                                            <posts user='example' dt='2008-12-11T08:00:00Z' tag=''>
                                                <post href='http://example.com/' 
                                                 hash='a5a6f3d28d8dd549f3cad39fb0b34104' 
                                                 description='Example domain' 
                                                 tag='example domain' 
                                                 time='2008-12-12T07:45:52Z' 
                                                 extended='An example site.' 
                                                 meta='762ee1d713648596931f798a7ba987e0'/>

                                                <post href='http://second-example.com/' 
                                                 hash='ce67c6fbe4f79a521481060e2447001b' 
                                                 description='Another example domain' 
                                                 tag='example domain another' 
                                                 time='2008-12-12T04:04:24Z' 
                                                 extended='Another example site.' 
                                                 meta='fa2a46d239ad4f089c3ce7961d958b2e'/>
                                            </posts>
                                            <!-- fe03.api.del.ac4.yahoo.net uncompressed/chunked Sun Dec 14 20:20:32 PST 2008 -->";
        private Post EXAMPLE_POST = new Post(
            "http://example.com/",
            "a5a6f3d28d8dd549f3cad39fb0b34104",
            "Example domain",
            "example domain",
            "An example site.",
            "2008-12-12T07:45:52Z",
            "762ee1d713648596931f798a7ba987e0");
        private Post ANOTHER_EXAMPLE_POST = new Post(
            "http://second-example.com/",
            "ce67c6fbe4f79a521481060e2447001b",
            "Another example domain",
            "example domain another",
            "Another example site.",
            "2008-12-12T04:04:24Z",
            "fa2a46d239ad4f089c3ce7961d958b2e");


        public DeliciousQueryProviderTest()
        {

        }

        [TestMethod]
        public void Execute()
        {
            MockRepository mocks = new MockRepository();

            WebClient client = mocks.StrictMock<WebClient>();
            IDelayer delayer = mocks.StrictMock<IDelayer>();
            IQueryTranslatorFactory factory = mocks.StrictMock<IQueryTranslatorFactory>();
            IQueryTranslator translator = mocks.StrictMock<IQueryTranslator>();

            Expression expression = Expression.Constant(new Object());

            string uri = "http://www.example.com";

            Byte[] documentBytes = new UTF8Encoding().GetBytes(DOCUMENT);
            Stream stream = new MemoryStream();
            stream.Write(documentBytes, 0, documentBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            DeliciousQueryProvider provider = new DeliciousQueryProvider(client, delayer, factory);

            // Set up the mocked call to Delay to actually execute the callback
            Expect.Call(delayer.Delay(null)).IgnoreArguments().Do((CallbackDelegate)delegate(Callback callback)
            {
                return callback();
            });

            Expect.Call(factory.Create(expression)).Return(translator);
            Expect.Call(translator.Translate()).Return(uri);
            Expect.Call(client.OpenRead(uri)).Return(stream);

            mocks.ReplayAll();

            object result = provider.Execute(expression);

            Assert.IsInstanceOfType(result, typeof(IEnumerable<Post>));
            
            IEnumerable<Post> posts = (IEnumerable<Post>)result;

            Assert.IsTrue(posts.Contains(EXAMPLE_POST), "Missing post for example.com.");
            Assert.IsTrue(posts.Contains(ANOTHER_EXAMPLE_POST), "Missing post for another-example.com.");

            mocks.VerifyAll();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
    }
}
