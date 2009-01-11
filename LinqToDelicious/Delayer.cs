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
        public int AdditionalDelay { get; set; }

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
