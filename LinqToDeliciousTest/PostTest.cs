using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LinqToDelicious;

namespace LinqToDeliciousTest
{
    /// <summary>
    /// Tests for Post
    /// </summary>
    [TestClass]
    public class PostTest
    {
        public PostTest()
        {

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
            String address = "http://example.com/";
            String hash = "a5a6f3d28d8dd549f3cad39fb0b34104";
            String description = "Example web page";
            String tags = "example testing domain test";
            String date = "2008-12-12T07:45:52Z";
            String extended = "These domain names are reserved for use in documentation and are not available for registration. See RFC 2606, Section 3.";
            String meta = "fa2a46d239ad4f089c3ce7961d958b2e";

            Post post = new Post(address, hash, description, tags, extended, date, meta);

            Assert.AreEqual(address, post.Address);
            Assert.AreEqual(hash, post.Hash);
            Assert.AreEqual(description, post.Description);

            Assert.AreEqual(4, post.Tags.Count);
            Assert.IsTrue(post.Tags.Contains("example"));
            Assert.IsTrue(post.Tags.Contains("testing"));
            Assert.IsTrue(post.Tags.Contains("domain"));
            Assert.IsTrue(post.Tags.Contains("test"));

            Assert.AreEqual(new DateTime(2008, 12, 12, 7, 45, 52), post.Date);
            Assert.AreEqual(extended, post.Extended);
            Assert.AreEqual(meta, post.Meta);
        }
    }
}
