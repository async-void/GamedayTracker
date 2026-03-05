using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.Services
{
    public sealed class DailyNumbersCacheService(IMemoryCache cache, ILogger<DailyNumbersCacheService> logger) : IDailyNumbersCache, IDailyNumbersRepository
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<DailyNumbersCacheService> _logger = logger;

        private static string UserKey(ulong guildId, ulong userId, DateOnly date)
            => $"DailyNumbers:{guildId}:{userId}:{date:yyyyMMdd}";

        private static string GuildKey(ulong guildId, DateOnly date)
            => $"DailyNumbers:{guildId}:{date:yyyyMMdd}";

        private readonly HashSet<(ulong GuildId, DateOnly Date)> _activeGuildKeys = [];

        // ------------------------------------------------------------
        // CHECK IF USER HAS SUBMITTED
        // ------------------------------------------------------------
        public bool HasUserSubmitted(ulong guildId, ulong userId, DateOnly date)
        {
            var key = UserKey(guildId, userId, date);
            return _cache.TryGetValue(key, out DailyNumberPick _);
        }

        // ------------------------------------------------------------
        // ADD USER PICK
        // ------------------------------------------------------------
        public void AddUserPick(DailyNumberPick pick)
        {
            var userKey = UserKey(pick.GuildId, pick.UserId, pick.Date);
            var guildKey = GuildKey(pick.GuildId, pick.Date);

            // Track that this guild has picks for this date
            _activeGuildKeys.Add((pick.GuildId, pick.Date));

            // Store individual user pick
            _cache.Set(userKey, pick, TimeSpan.FromDays(2));

            // Store in guild list
            var list = _cache.Get<List<DailyNumberPick>>(guildKey) ?? [];
            list.Add(pick);
            _cache.Set(guildKey, list, TimeSpan.FromDays(1));

            _logger.LogInformation("Stored daily numbers for {UserId} in guild {GuildId}", pick.UserId, pick.GuildId);
        }

        // ------------------------------------------------------------
        // GET ALL PICKS FOR GUILD
        // ------------------------------------------------------------
        public IReadOnlyList<DailyNumberPick> GetGuildPicks(ulong guildId, DateOnly date)
        {
            var key = GuildKey(guildId, date);
            return _cache.Get<List<DailyNumberPick>>(key) ?? [];
        }

        // ------------------------------------------------------------
        // GET USER PICKS FOR GUILD
        // ------------------------------------------------------------
        public DailyNumberPick? GetUserPicks(ulong guildId, ulong UserId, DateOnly date)
        {
            var key = UserKey(guildId, UserId, date);
            return _cache.Get<DailyNumberPick>(key);
        }

        // ------------------------------------------------------------
        // CLEAR AFTER SETTLEMENT
        // ------------------------------------------------------------
        public void ClearGuildPicks(ulong guildId, DateOnly date)
        {
            var guildKey = GuildKey(guildId, date);
            _cache.Remove(guildKey);

            _logger.LogInformation("Cleared daily numbers for guild {GuildId} on {Date}", guildId, date);
        }

        // ------------------------------------------------------------
        // GET ALL GUILDID'S
        // ------------------------------------------------------------
        public IReadOnlyList<ulong> GetActiveGuildIds(DateOnly date)
                => [.. _activeGuildKeys
                         .Where(k => k.Date == date)
                         .Select(k => k.GuildId)
                         .Distinct()];

        //------------------------------------------------------------
        // GET PICKS FOR DATE ASYNC
        //------------------------------------------------------------
        public Task<Result<IReadOnlyList<DailyNumberPick>, SystemError<DailyNumbersCacheService>>> GetPicksForDateAsync(DateOnly date, CancellationToken ct = default)
        {
            try
            {
                var allGuildKeys = GetActiveGuildIds(date); 

                var allPicks = new List<DailyNumberPick>();

                foreach (var guildId in allGuildKeys)
                {
                    var key = $"DailyNumbers:{guildId}:{date:yyyyMMdd}";
                    if (_cache.TryGetValue(key, out List<DailyNumberPick>? picks))
                    {
                        allPicks.AddRange(picks);
                    }
                }

                return Task.FromResult(
                    Result<IReadOnlyList<DailyNumberPick>, SystemError<DailyNumbersCacheService>>
                        .Ok(allPicks.AsReadOnly())
                );
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    Result<IReadOnlyList<DailyNumberPick>, SystemError<DailyNumbersCacheService>>
                        .Err(new SystemError<DailyNumbersCacheService>
                        {
                            ErrorCode = Guid.NewGuid(),
                            ErrorMessage = $"Failed to retrieve picks for date {date}: {ex.Message}",
                            CreatedBy = this,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.INFORMATION
                        }));
            }
        }

        // ------------------------------------------------------------
        // RESET CACHE FOR DATE
        // ------------------------------------------------------------
        public void ResetForDate(DateOnly date)
        {
            var guilds = _activeGuildKeys
                .Where(x => x.Date == date)
                .ToList();

            foreach (var (guildId, _) in guilds)
            {
                var guildKey = GuildKey(guildId, date);
                var picks = _cache.Get<List<DailyNumberPick>>(guildKey);

                if (picks != null)
                {
                    foreach (var pick in picks)
                    {
                        var userKey = UserKey(guildId, pick.UserId, date);
                        _cache.Remove(userKey);
                    }
                }

                _cache.Remove(guildKey);
                _activeGuildKeys.Remove((guildId, date));
            }

            _logger.LogInformation("DailyNumbers cache reset for {Date}", date);
        }

    }
}
