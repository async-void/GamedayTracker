using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public sealed class TicketIdGenerator
    {
        private static readonly object _lock = new();
        private static uint _counter = 0;

        public static ulong NextId()
        {
            lock (_lock)
            {
                ++_counter;
                return ((ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds() << 32) | _counter;
            }
        }
    }
}
