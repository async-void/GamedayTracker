using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models.DailyNumbers
{
    public sealed record DailyNumbersHistoryPickResult(ulong GuildId, DateOnly DrawDate, IReadOnlyList<int> WinningNumbers, string WinningType, DateTimeOffset DrawTimestamp)
    {
        public override string ToString()
        {
            return $"GuildId: {GuildId} | DrawDate: {DrawDate:MM-dd-yyyy} | WinningNumbers: [{string.Join(", ", WinningNumbers)}] | WinningType: {WinningType} | DrawTimestamp: {DrawTimestamp}";
        }
    }
    
}
