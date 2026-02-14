using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.Betting;
using GamedayTracker.Models.NFL;
using GamedayTracker.Utility;
using System.Security.Principal;

namespace GamedayTracker.Services
{
    public class BettingDataServiceProvider(IGameData gameData, IJsonDataService jsonDataService, IGuildMemberService memberService): IBetting
    {
        #region CAN AFFORD BET
        public Task<Result<BalanceCheckResult, SystemError<BettingDataServiceProvider>>> CanAffordBetAsync(Bank bank, decimal amount)
        {
            if (amount <= 0)
            {
                return Task.FromResult(
                    Result<BalanceCheckResult, SystemError<BettingDataServiceProvider>>
                        .Err(new SystemError<BettingDataServiceProvider>
                        {
                           ErrorMessage = "Bet amount must be greater than zero."
                        }
                ));
            }

            if (bank.Balance < amount)
            {
                return Task.FromResult(
                    Result<BalanceCheckResult, SystemError<BettingDataServiceProvider>>
                        .Err(new SystemError<BettingDataServiceProvider>
                        {
                            ErrorMessage = $"Insufficient funds. Your balance is {bank.Balance}."
                        }
                       
                ));
            }

            var result = new BalanceCheckResult
            {
                Allowed = true,
                Reason = "Sufficient balance."
            };

            return Task.FromResult(
                Result<BalanceCheckResult, SystemError<BettingDataServiceProvider>>
                    .Ok(result)
            );
        }
        #endregion

        #region PLACE BET
        public async Task<Result<Bet, SystemError<BettingDataServiceProvider>>> PlaceBet(Bet bet)
        {
           
            throw new NotImplementedException();
        }
        #endregion

        #region CALCULATE PAYOUT
        public Result<BetPayoutResult, SystemError<BettingDataServiceProvider>> CalculatePayout( Bet bet, int americanOdds)
        {
            if (bet.WagerAmount <= 0)
                return new SystemError<BettingDataServiceProvider>
                {
                    ErrorMessage = "Bet amount must be greater than zero.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this
                };

            // 1. Convert odds → multiplier
            var multiplier = GetMultiplierFromAmericanOdds(americanOdds);

            // 2. Calculate winnings
            var winnings = bet.WagerAmount * (multiplier - 1m);

            // 3. Total payout includes original stake
            var total = bet.WagerAmount + winnings;

            var result = new BetPayoutResult
            {
                Multiplier = multiplier,
                Winnings = winnings,
                TotalPayout = total
            };

            return result;
        }
        #endregion

        #region GET MULTIPLIER FROM AMERICAN ODDS
        public decimal GetMultiplierFromAmericanOdds(int odds)
        {
            if (odds > 0)
                return 1m + (odds / 100m);

            return 1m + (100m / Math.Abs(odds));
        }
        #endregion
    }
}
