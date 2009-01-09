using LinqToDelicious;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using IQ;
using Rhino.Mocks;
using System.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LinqToDeliciousTest
{
    /// <summary>
    ///This is a test class for QueryTranslatorTest and is intended
    ///to contain all QueryTranslatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueryTranslatorTest
    {
        private MockRepository mMocks = new MockRepository();
        private ParameterExpression mParam = Expression.Parameter(typeof(Post), "post");

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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// A test for date equality clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date = 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereDateEqual()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.Equal(left, right));
            string url = TranslateQuery(mParam, dateClause);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("fromdt=1/1/2008 12:00:00 AM"));
            Assert.IsTrue(url.Contains("todt=1/1/2008 12:00:00 AM"));
        }

        /// <summary>
        /// A test for date less than clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date &lt; 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereDateLessThan()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.LessThan(left, right));
            string url = TranslateQuery(mParam, dateClause);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("todt=1/1/2008 12:00:00 AM"));
            Assert.IsFalse(url.Contains("fromdt="));
        }

        /// <summary>
        /// A test for date less than/equal clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date &lt;= 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereDateLessThanOrEqual()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.LessThanOrEqual(left, right));
            string url = TranslateQuery(mParam, dateClause);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("todt=1/2/2008 12:00:00 AM"));
            Assert.IsFalse(url.Contains("fromdt="));
        }

        /// <summary>
        /// A test for date greater than clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date &gt; 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereDateGreaterThan()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.GreaterThan(left, right));
            string url = TranslateQuery(mParam, dateClause);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("fromdt=1/1/2008 12:00:00 AM"));
            Assert.IsFalse(url.Contains("todt="));
        }

        /// <summary>
        /// A test for date greater than/equal clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date &gt;= 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereDateGreaterOrThanEqual()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.GreaterThanOrEqual(left, right));
            string url = TranslateQuery(mParam, dateClause);

            Debug.WriteLine("url: " + url);
            
            Assert.IsTrue(url.Contains("fromdt=12/31/2007 12:00:00 AM"));
            Assert.IsFalse(url.Contains("todt="));
        }

        /// <summary>
        /// A test for tag clauses.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Tags.Contains("example")))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereTags()
        {
            Expression tagClause = BuildTagClause(mParam, "example");
            string url = TranslateQuery(mParam, tagClause);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("tag=example"));
        }

        /// <summary>
        /// A test for date and tag clauses together.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Tags.Contains("example") && post.Date &gt; 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereTagsAndDateGreaterThan()
        {
            Expression tagClause = BuildTagClause(mParam, "example");
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.GreaterThan(left, right));
            Expression tagAndDateClauses = Expression.And(tagClause, dateClause);

            string url = TranslateQuery(mParam, tagAndDateClauses);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("tag=example"));
            Assert.IsTrue(url.Contains("fromdt=1/1/2008 12:00:00 AM"));
            Assert.IsFalse(url.Contains("todt="));
        }

        /// <summary>
        /// A test for date and tag clauses together.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Tags.Contains("example") && post.Date = 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        public void WhereTagsAndDateEquals()
        {
            Expression tagClause = BuildTagClause(mParam, "example");
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.Equal(left, right));
            Expression tagAndDateClauses = Expression.And(tagClause, dateClause);

            string url = TranslateQuery(mParam, tagAndDateClauses);

            Debug.WriteLine("url: " + url);

            Assert.IsTrue(url.Contains("tag=example"));

            Assert.IsTrue(url.Contains("fromdt=1/1/2008 12:00:00 AM"));
            Assert.IsTrue(url.Contains("todt=1/1/2008 12:00:00 AM"));
        }

        /// <summary>
        /// A test for an NotSupportedException when trying to OR in the where clause.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Tags.Contains("example") || post.Date = 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        [ExpectedException(typeof(NotSupportedException))]
        public void UnsupportedWhereClause()
        {
            Expression tagClause = BuildTagClause(mParam, "example");
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.Equal(left, right));
            Expression tagAndDateClauses = Expression.Or(tagClause, dateClause);

            TranslateQuery(mParam, tagAndDateClauses);
        }

        /// <summary>
        /// A test for an NotSupportedException when trying to compare dates with !=.
        ///
        /// Query(LinqToDelicious.Post).Where(post => (post.Date != 1/1/2008 12:00:00 AM))
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToDelicious.dll")]
        [ExpectedException(typeof(NotSupportedException))]
        public void UnsupportedDateComparison()
        {
            Expression dateClause = BuildDateClause(mParam, new DateTime(2008, 1, 1), (left, right) => Expression.NotEqual(left, right));

            TranslateQuery(mParam, dateClause);
        }

        private Expression BuildTagClause(ParameterExpression lambdaParameter, String tag)
        {
            // post.Tags
            MemberExpression member = Expression.Property(lambdaParameter, "Tags");

            // "example"
            ConstantExpression tagExpression = Expression.Constant(tag);

            // post.Tags.Contains("Example")
            MethodCallExpression containsCall = Expression.Call(
                member,
                typeof(List<String>).GetMethod("Contains", new Type[] { typeof(String) }),
                tagExpression);

            return containsCall;
        }

        private Expression BuildDateClause(ParameterExpression lambdaParameter, DateTime date, Func<Expression, Expression, Expression> comparator)
        {
            //IQueryable<Post> queryable = mocks.StrictMock<IQueryable<Post>>();
            // ConstantExpression queryableExpression = Expression.Constant(queryable);
            // SetupResult.For(queryable.Expression).Return(queryableExpression);
            // Debug.WriteLine(queryable.Expression);

            // post.Date
            Expression left = Expression.Property(lambdaParameter, "Date");

            // <evaluated comparedDate>
            Expression right = Expression.Constant(date);

            // post.Date == <evaluated comparedDate>
            Expression comparison = comparator(left, right);

            return comparison;
        }

        private string TranslateQuery(ParameterExpression lambdaParameter, Expression clauses)
        {
            // from post in queryable
            // where post.Date == <evaluated comparedDate>

            // i.e. queryable.Where(post => post.Date == <evaluated comparedDate>)

            // TODO: This should probably be mocked instead of using a meaningless array.
            // See the TODO below - it probably relates.
            IQueryable<Post> queryable = new Post[] { }.AsQueryable<Post>();

            // queryable.Where( ... )
            MethodCallExpression whereCall = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { typeof(Post) },
                queryable.Expression,
                Expression.Lambda<Func<Post, bool>>(clauses, new ParameterExpression[] { lambdaParameter }));

            // TODO: Double check that queryable.Expression gives us what we want above
            // Debug output says it results in a Post[], but I thought it should be an IQueryable<Post>
            // This argument isn't used in the translator at the moment.

            QueryTranslator translator = new QueryTranslator(whereCall);

            return translator.Translate();
        }
    }
}
