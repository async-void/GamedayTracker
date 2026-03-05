using GamedayTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Bet
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ulong UserId { get; set; }
        public string EventId { get; set; } = ""; // ESPN game ID
        public DateTimeOffset GameDate { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        public decimal WagerAmount { get; set; }
        public decimal Multiplier { get; set; }

        // What the user bet on
        public BetType Type { get; set; }
        public string Selection { get; set; } = ""; // e.g. "KC", "SF", "Over", "Under", "KC -3.5"
        public int Odds { get; set; }

        // Outcome
        public BetStatus Status { get; set; } = BetStatus.Pending;
        public decimal? Payout { get; set; }

    }
}
