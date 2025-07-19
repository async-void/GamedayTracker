using System.ComponentModel.DataAnnotations;

namespace GamedayTracker.Models
{
    public class Guild
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string GuildId { get; set; }
        public string? NotificationChannelId { get; set; }
        public required string GuildName { get; set; }
        public string? GuildOwnerId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public bool IsDailyHeadlinesEnabled { get; set; }
        public bool IsRealTimeScoresEnabled { get; set; }
        public bool ReceiveSystemMessages { get; set; }
    }
}
