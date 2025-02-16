using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace GamedayTracker.Models
{
    public class GuildMember
    {
        public int Id { get; set; }
        public required string MemberName { get; set; }
        public required string MemberId { get; set; }
        public required string GuildId { get; set; }
        public string? FavoriteTeam { get; set; }
        public int PoolWins { get; set; }

        //Navigational Properties
        public int PlayerPicksId { get; set; }
        public PlayerPicks? PlayerPicks { get; set; }
        public int BankId { get; set; }
        public Bank? Bank { get; set; }
        public virtual ICollection<Bet>? BetHistory { get; set; }
    }
}
