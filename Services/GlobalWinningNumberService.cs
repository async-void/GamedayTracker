using GamedayTracker.Enums;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.DailyNumbers;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            if (_store.TryGetValue(date, out _))
                _store.Clear();
               
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

        public async Task<Result<ConcurrentDictionary<DateOnly, int[]>, SystemError<GlobalWinningNumberService>>> GetLotteryHistory()
        {
            //TODO: fix me - this would likely pull from the json rather than an in-memory dictionary
            if (_store.IsEmpty) 
            {
                return Result<ConcurrentDictionary<DateOnly, int[]>, SystemError<GlobalWinningNumberService>>.Err(
                    new SystemError<GlobalWinningNumberService>
                    {
                        ErrorMessage = "No lottery history found",
                        ErrorCode = Guid.NewGuid(),
                        ErrorType = ErrorType.INFORMATION,
                    });
            }
            return Result<ConcurrentDictionary<DateOnly, int[]>, SystemError<GlobalWinningNumberService>>.Ok(_store);
        }

        public async Task<Result<bool, SystemError<GlobalWinningNumberService>>> SaveWinningNumberToJsonAsync(DateOnly date, int[] numbers, int winCount)
        {
            var year = date.Year;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"DailyNumbersHistory_{year}.json");
            var options = JsonHelper.DefaultJsonOptions;
            try
            {
                
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                if (!File.Exists(path))
                {
                   
                    var listOne = new List<DailyNumbersDrawingResult> { new(numbers, date, winCount) };
                    var json = JsonSerializer.Serialize(listOne, options);
                    await File.WriteAllTextAsync(path, json);
                    return Result<bool, SystemError<GlobalWinningNumberService>>.Ok(true);
                }

                var existingJson = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<DailyNumbersDrawingResult>>(existingJson, options) ?? [];
                list.Add(new DailyNumbersDrawingResult(numbers, date, winCount));
               
                var updatedJson = JsonSerializer.Serialize(list, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<GlobalWinningNumberService>>.Ok(true);
            }
            catch (Exception ex)
            {
                return Result<bool, SystemError<GlobalWinningNumberService>>.Err(
                    new SystemError<GlobalWinningNumberService>
                    {
                        ErrorMessage = $"Failed to save winning numbers to JSON: {ex.Message}",
                        ErrorCode = Guid.NewGuid(),
                        ErrorType = ErrorType.WARNING,
                    });
            }
        }

        public void ResetWinningNumbers(DateOnly date)
        {
            _store.TryRemove(date, out _);
            return;
        }
    }

}
