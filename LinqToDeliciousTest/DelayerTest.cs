using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using LinqToDelicious;

namespace LinqToDeliciousTest
{
    /// <summary>
    /// Summary description for DelayerTest
    /// </summary>
    [TestClass]
    public class DelayerTest
    {
        // Acceptable error in measured call times
        private const double TIME_DIFFERENCE = 0.1;

        // Acceptable error in measured call times for "immediate" results
        private const double IMMEDIATE_TIME_DIFFERENCE = 0.01;

        public DelayerTest()
        {
        
        }

        [TestMethod]
        public void Immediate500MillisecondDelay()
        {
            TestImmediateDelay(new Delayer(500), 500);
        }

        [TestMethod]
        public void Immediate100MillisecondDelay()
        {
            TestImmediateDelay(new Delayer(100), 100);
        }

        [TestMethod]
        public void Subsequent500MillisecondDelay()
        {
            TestSubsequentDelay(new Delayer(500), 500);
        }

        [TestMethod]
        public void Subsequent100MillisecondDelay()
        {
            TestSubsequentDelay(new Delayer(100), 100);
        }

        [TestMethod]
        public void AdditionalImmediateDelay()
        {
            Delayer delayer = new Delayer(100);
            delayer.AdditionalDelay = 100;

            TestImmediateDelay(delayer, 200);
        }

        [TestMethod]
        public void AdditionalSubsequentDelay()
        {
            Delayer delayer = new Delayer(100);
            delayer.AdditionalDelay = 100;

            TestSubsequentDelay(delayer, 100);
        }

        private void TestImmediateDelay(Delayer delayer, int delay)
        {
            ManualResetEvent manualEvent = new ManualResetEvent(false);

            bool called = false;
            DateTime timeCalled = DateTime.MaxValue;
            Object obj = new Object();

            Callback callback = delegate()
            {
                called = true;
                timeCalled = DateTime.Now;
                manualEvent.Set();

                return obj;
            };

            DateTime timeStarted = DateTime.Now;
            Object result = delayer.Delay(callback);

            manualEvent.WaitOne(delay * 2, false);

            int timeElapsed = (int)(timeCalled - timeStarted).TotalMilliseconds;

            Assert.IsTrue(called);
            Assert.AreSame(obj, result);

            if (timeElapsed > delay * IMMEDIATE_TIME_DIFFERENCE)
            {
                Assert.Inconclusive("Test failed, or delay was too small. Time elapsed was " + timeElapsed + " msec; expected <= " + (delay * IMMEDIATE_TIME_DIFFERENCE));
            }
        }

        private void TestSubsequentDelay(Delayer delayer, int delay)
        {
            ManualResetEvent manualEvent = new ManualResetEvent(false);

            bool called = false;
            DateTime timeCalled = DateTime.MaxValue;
            Object obj = new Object();

            Callback nop = delegate()
            {
                return null;
            };

            Callback callback = delegate()
            {
                called = true;
                timeCalled = DateTime.Now;
                manualEvent.Set();
                return obj;
            };

            // This is the difference between this method and TestImmediateDelay.
            // This method ignores the first call to Delay and tests the delay of the
            // second call to Delay.
            delayer.Delay(nop);

            DateTime timeStarted = DateTime.Now;

            Object result = delayer.Delay(callback);

            manualEvent.WaitOne(delay * 2, false);


            int timeElapsed = (int)(timeCalled - timeStarted).TotalMilliseconds;
            double elapsedError = delay * TIME_DIFFERENCE;

            Assert.IsTrue(called);
            Assert.AreSame(obj, result);

            Assert.IsTrue(timeElapsed >= delay - elapsedError,
                "Delay was not at least (" + delay + " - " + elapsedError + ") msec; total time elapsed was " + timeElapsed);

            if (timeElapsed > delay + elapsedError)
            {
                Assert.Inconclusive("Test failed, or specified delay was too small to be supported. Time elapsed was " + timeElapsed + " msec; expected <= " + elapsedError);
            }
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
