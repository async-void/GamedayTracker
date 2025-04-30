using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Extensions
{
    public static class NumericExtension
    {
        #region TO INT
        public static int ToInt(this string str)
        {
            return int.TryParse(str, out var result) ? result : 0;
        }
        #endregion

        #region TO DOUBLE
        public static double ToDouble(this string str)
        {
            return double.TryParse(str, out var result) ? result : 0;
        }
        #endregion
    }
}
