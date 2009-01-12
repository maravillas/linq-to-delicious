using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Threading;

namespace LinqToDelicious
{
    class Delayer : IDelayer
    {
        /// <summary>
        /// The number of milliseconds to delay each time Delay is called.
        /// </summary>
        public int DelayLength { get; private set; }

        /// <summary>
        /// The number of milliseconds to delay the only next time Delay is called.
        /// </summary>
        public int AdditionalDelay { get; set; }

        private Callback mCallback;
        private DateTime mLastCall;

        /// <summary>
        /// Creates a new Delayer.
        /// </summary>
        /// <param name="delay"></param>
        public Delayer(int delay)
        {
            DelayLength = delay;
            mLastCall = DateTime.MaxValue;
        }

        /// <summary>
        /// Suspends the current thread by the previously specified length of time, 
        /// and executes the specified callback when finished.
        /// </summary>
        /// <param name="callback">The Callback to be executed when the delay is complete.</param>
        /// <returns>The result of the callback.</returns>
        public object Delay(Callback callback)
        {
            int timeDifference = (int)(DateTime.Now - mLastCall).TotalMilliseconds;
            int delay = Math.Max((DelayLength + Math.Max(AdditionalDelay, 0)) - timeDifference, 0);

            mCallback = callback;

            Debug.WriteLine("Delaying for " + delay + " msec");

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }

            mLastCall = DateTime.Now;
            AdditionalDelay = 0;

            return mCallback();
        }
    }
}
