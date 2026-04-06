using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Extensions
{
    public static class DateOnlyExtension
    {
        public static DateOnly FormatDate(this string input)
        {
            string[] formats =
            {
                "M-d-yyyy",
                "M/d/yyyy",
                "MM-dd-yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "yyyyMMdd"
            };

            if (DateOnly.TryParseExact(input, formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }

            throw new FormatException($"Input '{input}' was not in a recognized date format.");
        }
    }
}
