using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDailyNumbersCache
    {
        bool HasUserSubmitted(ulong guildId, ulong userId, DateOnly date);
        Task AddUserPick(DailyNumberPick pick);
        IReadOnlyList<DailyNumberPick> GetGuildPicks(ulong guildId, DateOnly date);
        IReadOnlyList<ulong> GetActiveGuildIds(DateOnly date);
        DailyNumberPick? GetUserPicks(ulong guildId, ulong UserId, DateOnly date);
        void ClearGuildPicks(ulong guildId, DateOnly date);
        void ResetForDate(DateOnly date);
    }
}
