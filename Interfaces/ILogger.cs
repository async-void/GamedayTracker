using DSharpPlus.Entities;
using GamedayTracker.Enums;

namespace GamedayTracker.Interfaces
{
    public interface ILogger
    {
        void Log(LogTarget target, LogType type, DateTimeOffset timestamp, string message);
    }
}
