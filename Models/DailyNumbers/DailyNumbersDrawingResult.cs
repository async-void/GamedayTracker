using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models.DailyNumbers
{
    public sealed record DailyNumbersDrawingResult (int[] WinningNumbers, DateOnly DrawDate, int WinnerCount)
    {
        public override string ToString()
        {
            var numbers = string.Join(", ", WinningNumbers);
            return $"Draw Date: {DrawDate}, Winning Numbers: {numbers} Winners: {WinnerCount}";
        }
    }
}
