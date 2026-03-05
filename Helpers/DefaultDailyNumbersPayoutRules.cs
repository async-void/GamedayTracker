using GamedayTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public sealed class DefaultDailyNumbersPayoutRules : IDailyNumbersPayoutRules
    {
        public decimal GetPayout(int matchCount) =>
        matchCount switch
        {
            3 => 1000m,
            2 => 50m,
            1 => 5m,
            _ => 0m
        };

    }
}
