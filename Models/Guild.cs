using System.ComponentModel.DataAnnotations;

namespace GamedayTracker.Models
{
    public class Guild
    {
        public Guid Id { get; set; }
        public required long GuildId { get; set; }
        public long? NotificationChannelId { get; set; }
        public required string GuildName { get; set; }
        public long? GuildOwnerId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
    }
}
