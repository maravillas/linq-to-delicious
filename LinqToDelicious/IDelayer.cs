using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToDelicious
{
    internal delegate object Callback();

    internal interface IDelayer
    {
        int AdditionalDelay { get; set; }

        object Delay(Callback callback);
    }
}
