using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToDelicious
{
    internal delegate object Callback();

    internal interface IDelayer
    {
        /// <summary>
        /// The number of milliseconds to delay each time Delay is called.
        /// </summary>
        int DelayLength { get; }

        /// <summary>
        /// The number of milliseconds to delay the only next time Delay is called.
        /// </summary>
        int AdditionalDelay { get; set; }

        /// <summary>
        /// Suspends the current thread by the previously specified length of time, 
        /// and executes the specified callback when finished.
        /// </summary>
        /// <param name="callback">The Callback to be executed when the delay is complete.</param>
        /// <returns>The result of the callback.</returns>
        object Delay(Callback callback);
    }
}
