using DSharpPlus;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Pagination
{
    public class MemberBetsPaginationData
    {
        public required IReadOnlyList<Bet> Bets { get; init; } = [];
        public required DiscordClient DiscordClient { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
    }
}
