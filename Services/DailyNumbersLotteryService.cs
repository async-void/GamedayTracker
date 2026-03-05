using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class DailyNumbersLotteryService : ILotteryService
    {
        public Task<Result<int[], SystemError<DailyNumbersLotteryService>>> DrawDailyNumbers()
        {
            var number = Random.Shared.Next(0, 1000);
            var numbers = new int[3]
            {
                number / 100,
                (number / 10) % 10,
                number % 10
            };

            return Task.FromResult(Result<int[], SystemError<DailyNumbersLotteryService>>.Ok(numbers));
        }
    }
}
