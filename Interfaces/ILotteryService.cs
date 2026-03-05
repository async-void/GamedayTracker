using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface ILotteryService
    {
        Task<Result<int[], SystemError<DailyNumbersLotteryService>>> DrawDailyNumbers();
    }
}
