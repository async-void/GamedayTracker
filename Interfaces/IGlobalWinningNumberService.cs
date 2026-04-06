using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IGlobalWinningNumberService
    {
        Task<Result<int[], SystemError<GlobalWinningNumberService>>> GenerateWinningNumberAsync(DateOnly date);
        Task<Result<int[], SystemError<GlobalWinningNumberService>>> GetWinningNumberAsync(DateOnly date);
        Task<Result<ConcurrentDictionary<DateOnly, int[]>, SystemError<GlobalWinningNumberService>>> GetLotteryHistory();
        Task<bool> HasWinningNumberAsync(DateOnly date);
        void ResetWinningNumbers(DateOnly date);
        Task<Result<bool, SystemError<GlobalWinningNumberService>>> SaveWinningNumberToJsonAsync(DateOnly date, int[] numbers, int winCount);
    }
}
