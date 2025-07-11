using GamedayTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IEvaluator
    {
        RealTimeScoresMode Evaluate(DateTimeOffset now);
        TimeSpan GetInterval(RealTimeScoresMode mode);
    }
}
