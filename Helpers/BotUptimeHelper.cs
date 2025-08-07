using GamedayTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public class BotUptimeHelper(IBotTimer botTimerService)
    {
        private readonly IBotTimer _botTimerService = botTimerService;
        public async Task<double> GetUptimePercentage(TimeSpan referenceWindow)
        {
            var timeStamp = await _botTimerService.GetTimestampAsync();
            TimeSpan uptime = DateTime.UtcNow - timeStamp.Value;

            // Clamp to 100% if uptime exceeds reference window
            double percentage = Math.Min(100.0, (uptime.TotalMilliseconds / referenceWindow.TotalMilliseconds) * 100);

            return Math.Round(percentage, 2); // Round to 2 decimal places
        }
    }
}
