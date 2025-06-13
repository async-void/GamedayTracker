using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class BotTimerDataServiceProvider: IBotTimer
    {
        public async Task<Result<bool, SystemError<BotTimerDataServiceProvider>>> WriteTimestampToTextAsync()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "timestamp.txt");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.Create(path).Dispose();
                await File.WriteAllTextAsync(path, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                return Result<bool, SystemError<BotTimerDataServiceProvider>>.Ok(true);
            }
            await File.WriteAllTextAsync(path, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
            return Result<bool, SystemError<BotTimerDataServiceProvider>>.Ok(true);
        }

        public async Task<Result<DateTime, SystemError<BotTimerDataServiceProvider>>> GetTimestampFromTextAsync()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "timestamp.txt");
            if (!File.Exists(path))
            {
                return Result<DateTime, SystemError<BotTimerDataServiceProvider>>.Err(new SystemError<BotTimerDataServiceProvider>
                {
                    ErrorMessage = "Timestamp file does not exist",
                    ErrorType = ErrorType.WARNING,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            var lines = await File.ReadAllLinesAsync(path);
            var timestamp = DateTime.Parse(lines.LastOrDefault() ?? DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            return Result<DateTime, SystemError<BotTimerDataServiceProvider>>.Ok(timestamp);
        }
    }
}
