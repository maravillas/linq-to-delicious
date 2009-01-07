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
        public int DelayLength { get; private set; }

        private Callback mCallback;
        private DateTime mLastCall;

        public Delayer(int delay)
        {
            DelayLength = delay;
            mLastCall = DateTime.MaxValue;
        }

        public object Delay(Callback callback)
        {
            int timeDifference = (int)(DateTime.Now - mLastCall).TotalMilliseconds;
            int delay = Math.Max(DelayLength - timeDifference, 0);

            mCallback = callback;

            Debug.WriteLine("Delaying for " + delay + " msec");

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }

            mLastCall = DateTime.Now;

            return mCallback();
        }
    }
}
