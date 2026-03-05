using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace GamedayTracker.Services
{
    public sealed class DailyNumbersPayoutService(IDailyNumbersRepository repository,IGlobalWinningNumberService winningNumberService,IDailyNumbersPayoutRules payoutRules,
        ILogger<DailyNumbersPayoutService> logger, IJsonDataService jsonDataService): IDailyNumbersPayoutService
    {
        public async Task<Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>> CalculateWinnersAsync(DateOnly date, CancellationToken ct = default)
        {
            var winningNumberResult = await winningNumberService.GetWinningNumberAsync(date);
            if (!winningNumberResult.IsOk || winningNumberResult.Value.Length <= 0)
                return Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>
                    .Err(new SystemError<DailyNumbersPayoutService>
                    {
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"Failed to retrieve winning number for {date}: {winningNumberResult.Error.ErrorMessage}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION
                    });

            var picksResult = await repository.GetPicksForDateAsync(date, ct);
            if (!picksResult.IsOk || picksResult.Value.Count <= 0)
                return Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>
                    .Err(new SystemError<DailyNumbersPayoutService>
                    {
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"Failed to retrieve winning picks for {date}: {winningNumberResult.Error.ErrorMessage}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION
                    });

            var winningNumber = winningNumberResult.Value;
            var picks = picksResult.Value;

            if (picks.Count == 0)
                return Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>
                    .Ok(Array.Empty<DailyNumbersResult>());

            var results = new List<DailyNumbersResult>(picks.Count);

            foreach (var pick in picks)
            {
                int matchCount = CountMatches(pick.Numbers, winningNumber);
                decimal payout = payoutRules.GetPayout(matchCount);

                logger.LogInformation(
                    "User {UserId} in Guild {GuildId} matched {MatchCount} numbers with payout {Payout}",
                    pick.UserId, pick.GuildId, matchCount, payout);

                results.Add(new DailyNumbersResult(
                    pick.UserId,
                    pick.GuildId,
                    pick.Numbers,
                    winningNumber,
                    matchCount,
                    payout,
                    pick.PlayType
                ));
            }

            return Result<IReadOnlyList<DailyNumbersResult>, SystemError<DailyNumbersPayoutService>>.Ok(results);
        }

        public async Task<Result<GuildMember, SystemError<DailyNumbersPayoutService>>> ApplyPayoutAsync(ulong userId,ulong guildId,decimal payout)
        {
            if (payout <= 0)
                return Result<GuildMember, SystemError<DailyNumbersPayoutService>>.Err(new SystemError<DailyNumbersPayoutService>
                 {
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"Invalid payout amount {payout} for user {userId} in guild {guildId}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION
                 });

            var memberResult = await jsonDataService.GetMemberFromJsonAsync(userId, guildId);
            if (!memberResult.IsOk)
            {
                return Result<GuildMember, SystemError <DailyNumbersPayoutService>>.Err(new SystemError<DailyNumbersPayoutService>
                    {
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"Failed to retrieve member {userId} in guild {guildId}: {memberResult.Error.ErrorMessage}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION
                    });
            }

            var member = memberResult.Value;
            member.Bank!.Balance += payout;
            member.Bank.DepositTimestamp = DateTimeOffset.UtcNow;

            var updateResult = await jsonDataService.UpdateMemberDataAsync(member);
            if (!updateResult.IsOk)
            {
                return Result<GuildMember, SystemError<DailyNumbersPayoutService>>.Err(new SystemError<DailyNumbersPayoutService>
                {
                    ErrorCode = Guid.NewGuid(),
                    ErrorMessage = $"Failed to update member {userId} in guild {guildId}: {memberResult.Error.ErrorMessage}",
                    CreatedBy = this,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION
                });
            }

            return Result<GuildMember, SystemError<DailyNumbersPayoutService>>.Ok(member);
        }

        private static int CountMatches(IReadOnlyList<int> pick, int[] winning)
        {
            int matches = 0;
            for (int i = 0; i < 3; i++)
            {
                if (winning.Contains(pick[i]))
                    matches++;
            }
            return matches;
        }
    }
}
