using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class DmPayload
    {
        public ulong UserId { get; init; }
        public string Message { get; init; } = string.Empty;

        public int[]? UserPick { get; init; }
        public int[]? WinningNumbers { get; init; }
        public decimal? Payout { get; init; }
        public string? PlayType { get; init; }

    }
}
