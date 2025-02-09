using GamedayTracker.Enums;

namespace GamedayTracker.Utility
{
    public static class TimestampExtensions
    {
        public static string ToTimestamp(this DateTimeOffset dto, TimestampFormat format = TimestampFormat.Relative)
            => $"<t:{dto.ToUnixTimeSeconds()}:{(char)format}>";
    }
}
