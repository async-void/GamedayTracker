using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public required GuildMember GuildMember { get; set; }
        public int GuildMemberId { get; set; }
        public string? AwayTeamName { get; set; }
        public string? HomeTeamName { get; set; }
        public double BetAmount { get; set; }

    }
}
