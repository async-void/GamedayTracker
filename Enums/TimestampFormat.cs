using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Enums
{
    public enum TimestampFormat : byte
    {
        Relative = (byte)'R',
        ShortDate = (byte)'d',
        LongDate = (byte)'D',
        ShortTime = (byte)'t',
        LongTime = (byte)'T',
        ShortDateTime = (byte)'f',
        LongDateTime = (byte)'F',
    }
}
