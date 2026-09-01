using DSharpPlus.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamedayTracker.Models
{
    public class Guild
    {
        public ulong GuildId { get; set; }
        public string? NotificationChannelId { get; set; }
        public required string GuildName { get; set; }
        public ulong GuildOwnerId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public bool IsDailyHeadlinesEnabled { get; set; }
        public bool IsRealTimeScoresEnabled { get; set; }
        public bool ReceiveSystemMessages { get; set; }

        [NotMapped]
        public Dictionary<ulong, DiscordMember>? DiscordMembers { get; set; }
    }
}
