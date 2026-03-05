using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDailyNumbersPayoutService
    {
        Task<Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>> CalculateWinnersAsync(DateOnly date, CancellationToken ct = default);
        Task<Result<GuildMember, SystemError<DailyNumbersPayoutService>>> ApplyPayoutAsync(ulong userId, ulong guildId, decimal payout);
    }
}
