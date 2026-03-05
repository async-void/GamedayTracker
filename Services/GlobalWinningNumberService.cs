using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System.Collections.Concurrent;

namespace GamedayTracker.Services
{
    public class GlobalWinningNumberService(ILotteryService lottery) : IGlobalWinningNumberService
    {
        private readonly ILotteryService _lottery = lottery;
        private readonly ConcurrentDictionary<DateOnly, int[]> _store = new();

        public Task<bool> HasWinningNumberAsync(DateOnly date)
            => Task.FromResult(_store.ContainsKey(date));

        public async Task<Result<int[], SystemError<GlobalWinningNumberService>>> GenerateWinningNumberAsync(DateOnly date)
        {
            if (_store.TryGetValue(date, out var existing))
                return Result<int[], SystemError<GlobalWinningNumberService>>.Ok(existing);

            var draw = await _lottery.DrawDailyNumbers();
            if (!draw.IsOk)
            {
                return Result<int[], SystemError<GlobalWinningNumberService>>.Err(
                    new SystemError<GlobalWinningNumberService>
                    {
                        ErrorMessage = $"Failed to draw winning number: {draw.Error.ErrorMessage}",
                        ErrorCode = Guid.NewGuid(),
                        ErrorType = ErrorType.INFORMATION,
                    });
            }

            _store[date] = draw.Value;
            return Result<int[], SystemError<GlobalWinningNumberService>>.Ok(draw.Value);
        }

        public async Task<Result<int[], SystemError<GlobalWinningNumberService>>> GetWinningNumberAsync(DateOnly date)
        {
            if (_store.TryGetValue(date, out var number))
                return Result<int[], SystemError<GlobalWinningNumberService>>.Ok(number);

            return Result<int[], SystemError<GlobalWinningNumberService>>.Err(
                    new SystemError<GlobalWinningNumberService>
                    {
                        ErrorMessage = $"No winning number found for {date}",
                        ErrorCode = Guid.NewGuid(),
                        ErrorType = ErrorType.INFORMATION,
                    });

        }
    }

}
