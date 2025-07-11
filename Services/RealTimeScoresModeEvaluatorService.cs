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
            if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 13 && now.Hour <= 23)
                return RealTimeScoresMode.LiveGame;

            if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour >= 11 && now.Hour < 13)
                return RealTimeScoresMode.PreGame;

            return RealTimeScoresMode.Offseason;
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
