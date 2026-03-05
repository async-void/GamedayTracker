using DSharpPlus;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GamedayTracker.Jobs
{
    public class DailyNumbersLotteryJob(ILogger<DailyNumbersLotteryJob> logger, IGlobalWinningNumberService winningService,
         IDailyNumbersPayoutService payoutService, IDmQueue dmQueue, IDailyNumbersCache cacheService, IJsonDataService jsonService) : IJob
    {
        private readonly ILogger<DailyNumbersLotteryJob> _logger = logger;
        private readonly IGlobalWinningNumberService _winningNumberService = winningService;
        private readonly IDailyNumbersPayoutService _payoutService = payoutService;
        private readonly IDailyNumbersCache _chacheService = cacheService;
        private readonly IJsonDataService _jsonDataService = jsonService;
        private readonly IDmQueue _dmQueue = dmQueue;

        public async Task Execute(IJobExecutionContext context)
        {
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var numbersResult = await _winningNumberService.GenerateWinningNumberAsync(date);
            if (!numbersResult.IsOk)
            {
                _logger.LogError("Failed to generate winning number for Daily Numbers Lottery: {Error}", numbersResult.Error.ErrorMessage);
                return;
            }

            var winResult = await _payoutService.CalculateWinnersAsync(date);
            if (!winResult.IsOk)
            {
                _logger.LogError("Failed to calculate winners for Daily Numbers Lottery: {Error}", winResult.Error.ErrorMessage);
                return;
            }

            foreach (var winner in winResult.Value)
            {
                _logger.LogInformation("Daily Numbers Drawing Result: winners:{winners} Player Numbers:{playerNumbers} Lottery Numbers:{lotteryNumbers}", winResult.Value.Count,
                    string.Join(", ", winner.Pick), string.Join(", ", numbersResult.Value));
                if (winner.Payout <= 0)
                    continue;

                _dmQueue.Enqueue(new DmPayload
                {
                    UserId = winner.UserId,
                    Message = $"Congratulations! You won {winner.Payout:C} for Daily Numbers on {date:MM/dd/yyy}, The winning numbers were {string.Join(", ", winner.WinningNumber)}.",
                    UserPick = [.. winner.Pick],
                    WinningNumbers = winner.WinningNumber,
                    Payout = winner.Payout,
                    PlayType = winner.PlayType,
                });

                //pay the winner
                var payoutResult = await _payoutService.ApplyPayoutAsync(winner.UserId, winner.GuildId, winner.Payout);
                if (!payoutResult.IsOk) {
                    _logger.LogError("Failed to apply payout for user {UserId} in Daily Numbers Lottery: {Error}", winner.UserId, payoutResult.Error.ErrorMessage);
                }
            }
            var guildsInPlay = _chacheService.GetActiveGuildIds(date);
            var guildPicks = _chacheService.GetGuildPicks(guildsInPlay[0], date);
            var playerPicks = guildPicks.SelectMany(gp => gp.Numbers).ToList();
            _logger.LogInformation("Daily Numbers: {numbers} | Player Picks {picks}", string.Join(", ", numbersResult.Value), string.Join(", ", playerPicks[0]));
            _logger.LogInformation("Daily Numbers settlement complete for {Date}. Winners: {count}, Winning numbers {winnumbers}", date, winResult.Value.Count, numbersResult.Value);
            _chacheService.ResetForDate(date);
        }
    }
}
