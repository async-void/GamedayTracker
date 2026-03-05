using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDailyNumbersRepository
    {
        Task<Result<IReadOnlyList<DailyNumberPick>, SystemError<DailyNumbersCacheService>>>GetPicksForDateAsync(DateOnly date, CancellationToken ct = default);
    }
}
