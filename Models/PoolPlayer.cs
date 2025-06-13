using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class PoolPlayer
    {
        public int Id { get; set; }
        public ulong PlayerId { get; set; }
        public required string PlayerName { get; set; }
        public required string Company { get; set; }
        public double Balance { get; set; }
        public DateTime DepositTimestamp { get; set; }
        public string[]? Picks { get; set; }

        public int? GuildMemberId { get; set; }
        public GuildMember? GuildMember { get; set; }
    }
}
