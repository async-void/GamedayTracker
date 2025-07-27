using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bet
    {
        public Guid BetId { get; set; }
        public required string MemberId { get; set; }
        public required string GuildId { get; set; }
        public double BetAmount { get; set; }
        public required Matchup Matchup { get; set; }
    }
}
