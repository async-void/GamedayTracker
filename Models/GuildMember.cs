using GamedayTracker.Models.DailyNumbers;
using GamedayTracker.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace GamedayTracker.Models
{
    public class GuildMember
    {
        [Key]
        public ulong MemberId { get; set; } 
        public ulong GuildId { get; set; }
        public required string MemberName { get; set; }
        public required string GuildName { get; set; }
        public string? FavoriteTeam { get; set; }
        public int BetWins { get; set; }
        public List<DailyNumberPick>? DailyNumbers { get; set; }
        public List<Ticket> Tickets { get; set; } = [];
        public Bank? Bank { get; set; }
        public List<Bet> Bets { get; set; } = [];
    }
}
