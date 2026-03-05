using GamedayTracker.Enums;
using GamedayTracker.Models;
using GamedayTracker.Models.Betting;
using GamedayTracker.Models.NFL;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IBetting
    {
        Task<Result<BalanceCheckResult, SystemError<BettingDataServiceProvider>>> CanAffordBetAsync(Bank bank, decimal amount);
        Task<Result<Bet, SystemError<BettingDataServiceProvider>>> PlaceBet(Bet bet);
        decimal GetMultiplierFromAmericanOdds(int odds);
        Task<Result<IReadOnlyList<Bet>, SystemError<BettingDataServiceProvider>>> GetMemberBetsByIdAsync(ulong memberId, ulong guildId);
        Task<Result<IReadOnlyList<Bet>, SystemError<JsonDataServiceProvider>>> GetAllBetsAsync(BetType type);
    }
}
