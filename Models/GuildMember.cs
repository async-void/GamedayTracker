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
        public required DiscordMember Member { get; set; }
        public Team? FavoriteTeam { get; set; }

        //Navigational Properties
        public int BankId { get; set; }
        public int GuildMemberId { get; set; }
        public virtual ICollection<Bet>? BetHistory { get; set; }
    }
}
