using DSharpPlus;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Globalization;

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
            try
            {
                var eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                var easternNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, eastern);
                var date = DateOnly.FromDateTime(easternNow);
                _logger.LogInformation("Starting Daily Numbers Lottery Job for date: {Date}", date);
               
                var numbersResult = await _winningNumberService.GenerateWinningNumberAsync(date);

                if (!numbersResult.IsOk)
                {
                    _logger.LogError("Failed to generate winning number for Daily Numbers Lottery: {Error}", numbersResult.Error.ErrorMessage);
                    _chacheService.ResetForDate(date);
                    //_winningNumberService.ResetWinningNumbers(date);
                    return;
                }
                _logger.LogInformation("Generated winning numbers for Daily Numbers Lottery on {Date}: [{Numbers}]", date, string.Join(", ", numbersResult.Value));

                var winResult = await _payoutService.CalculateWinnersAsync(date.ToString("yyyyMMdd"));
                if (!winResult.IsOk)
                {
                    if (winResult.Error is not null)
                    {
                        _logger.LogError("Failed to calculate winners for Daily Numbers Lottery: | error: {Error} | Players: 0", winResult.Error.ErrorMessage);
                        _chacheService.ResetForDate(date);
                        //_winningNumberService.ResetWinningNumbers(date);
                        return;
                    }
                    else
                    {
                        _logger.LogError("Failed to calculate winners for Daily Numbers Lottery: players: 0");
                        _chacheService.ResetForDate(date);
                        // _winningNumberService.ResetWinningNumbers(date);
                        return;
                    }
                }

                try
                {
                    var _ = await _winningNumberService.SaveWinningNumberToJsonAsync(date, numbersResult.Value, winResult.Value.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving Daily Numbers Lottery results to JSON: {Error}", ex.Message);

                }

                if (winResult.Value.Count <= 0)
                {
                    _logger.LogInformation("No winners found for Daily Numbers Lottery on {Date}.", date);
                    _chacheService.ResetForDate(date);
                    // _winningNumberService.ResetWinningNumbers(date);
                    return;
                }
                foreach (var winner in winResult.Value)
                {
                    _logger.LogInformation("{date} Daily Numbers Drawing Result: winners:{winners} Player Numbers:{playerNumbers} Lottery Numbers:{lotteryNumbers}", date, winResult.Value.Count,
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
                    if (!payoutResult.IsOk)
                    {
                        _logger.LogError("Failed to apply payout for user {UserId} in Daily Numbers Lottery: {Error}", winner.UserId, payoutResult.Error.ErrorMessage);
                    }
                }
                _logger.LogInformation("Daily Numbers settlement complete for {Date}. Winners: {count}", date, winResult.Value.Count);
                _chacheService.ResetForDate(date);  
            }
            catch (Exception e)
            {
                var eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var date = DateOnly.FromDateTime(
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, eastern));
                _logger.LogError(e, "An error occurred while executing the Daily Numbers Lottery Job.\r\n{error}", e.Message);
                _chacheService.ResetForDate(date);
                //_winningNumberService.ResetWinningNumbers(date);
            }
        }
    }
}
