using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bank
    {
        public ulong BankId { get; set; }
        public decimal Balance { get; set; }
        public decimal LastDepositAmount { get; set; }
        public DateTimeOffset DepositTimestamp { get; set; }
        public ulong GuildMemberId { get; set; }

        [JsonIgnore]
        public GuildMember? GuildMember { get; set; }
    }
}
