using GamedayTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Utility.Multipliers
{
    public class BetMultiplier
    {
        private readonly Random _random = new();

        public decimal GetMultiplier(BetType betType)
        {
            return betType switch
            {
                BetType.Normal => RollTieredMultiplier(1.0m, 5.0m, 2.5m),
                BetType.Bonus => RollTieredMultiplier(5.0m, 20.0m, 10.0m),
                BetType.HighRoller => RollTieredMultiplier(10.0m, 100.0m, 20.0m),
                _ => 1.0m
            };
        }

        private decimal RollTieredMultiplier(decimal min, decimal max, decimal peakBias)
        {
            decimal range = max - min;
            decimal roll = (decimal)_random.NextDouble() * range;
            decimal skewDirection = _random.NextDouble() > 0.5 ? 0.8m : 0.2m;
            decimal skewed = peakBias + (roll - (peakBias - min)) * skewDirection;
            decimal clamped = Math.Clamp(skewed, min, max);
            return Math.Round(clamped, 2);
        }
    }
}
