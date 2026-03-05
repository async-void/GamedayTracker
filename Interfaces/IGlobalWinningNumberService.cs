using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
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
        Task<bool> HasWinningNumberAsync(DateOnly date);
    }
}
