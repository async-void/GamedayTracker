using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class RealTimeScoresModeEvaluatorService: IEvaluator
    {
        public RealTimeScoresMode Evaluate(DateTimeOffset now)
        {
            // NFL season months: September (9) through Febuary (2)
            bool inSeason = now.Month >= 9 || now.Month <= 2;

            if (!inSeason)
            {
                return RealTimeScoresMode.Offseason; // Febuary–August
            }

            // Explicit postseason handling (January–February)
            if (now.Month == 1 || now.Month == 2)
            {
                if (now.Day > 5) //Superbowl is over but still in Febuary
                    return RealTimeScoresMode.Offseason; 

                // Treat playoff/Super Bowl windows as LiveGame for freshness
                if ((now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 18 && now.Hour <= 23) ||
                    (now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 16 && now.Hour <= 23))
                {
                    return RealTimeScoresMode.LiveGame;
                }
                return RealTimeScoresMode.PostGame; // outside live windows
            }

            // Regular season logic (Sept–Dec)
            if ((now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 13 && now.Hour <= 23) ||
                (now.DayOfWeek == DayOfWeek.Thursday && now.Hour >= 18 && now.Hour <= 23) ||
                (now.DayOfWeek == DayOfWeek.Monday && now.Hour >= 20 && now.Hour <= 23))
            {
                return RealTimeScoresMode.LiveGame;
            }

            if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 11 && now.Hour < 13)
            {
                return RealTimeScoresMode.PreGame;
            }

            // In-season but not live/pre-game → PostGame
            return RealTimeScoresMode.PostGame;

        }


        public TimeSpan GetInterval(RealTimeScoresMode mode) => mode switch
        {
            RealTimeScoresMode.LiveGame => TimeSpan.FromSeconds(30),
            RealTimeScoresMode.PreGame => TimeSpan.FromMinutes(5),
            RealTimeScoresMode.PostGame => TimeSpan.FromHours(1),
            RealTimeScoresMode.Offseason => TimeSpan.FromHours(24),
            _ => TimeSpan.FromHours(24)
        };

    }
}
