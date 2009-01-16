using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToDelicious;
using System.Net;

namespace LinqToDeliciousTest
{
    /// <summary>
    /// Summary description for HttpWebRequestFactoryTest
    /// </summary>
    [TestClass]
    public class HttpWebRequestFactoryTest
    {
        public HttpWebRequestFactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
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

        [TestMethod]
        public void Create()
        {
            string username = "user";
            string password = "password";
            string uri = "http://www.example.com";

            HttpWebRequestFactory factory = new HttpWebRequestFactory(username, password);

            HttpWebRequest request = factory.Create(uri);

            NetworkCredential credentials = request.Credentials.GetCredential(new Uri(uri), "Basic");

            Assert.AreEqual(credentials.UserName, username);
            Assert.AreEqual(credentials.Password, password);

            Assert.IsTrue(request.UserAgent.Contains("LinqToDelicious"));
        }
    }
}
