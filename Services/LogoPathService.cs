using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class LogoPathService
    {
        public static Dictionary<string, string> LogoPaths { get; set; } = new()
        {
            { "default", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "img_default.png") },
            { "ARI", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "ARI.png") },
            { "ATL", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "ATL.png") },
            { "BAL", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "BAL.png") },
            { "BUF", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "BUF.png") },
            { "CAR", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "CAR.png") },
            { "CHI", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "CHI.png") },
            { "CIN", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "CIN.png") },
            { "CLE", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "CLE.png") },
            { "DAL", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "DAL.png") },
            { "DEN", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "DEN.png") },
            { "DET", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "DET.png") },
            { "GB", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "GB.png") },
            { "HOU", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "HOU.png") },
            { "IND", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "IND.png") },
            { "JAX", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "JAX.png") },
            { "KC", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "KC.png") },
            { "LAC", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "LAC.png") },
            { "LAR", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "LAR.png") },
            { "LV", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "LV.png") },
            { "MIA", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "MIA.png") },
            { "NE", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "NE.png") },
            { "NO", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "NO.png") },
            { "NYG", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "NYG.png") },
            { "NYJ", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "NYJ.png") },
            { "PHI", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "PHI.png") },
            { "PIT", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "PIT.png") },
            { "SEA", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "SEA.png") },
            { "SF", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "SF.png") },
            { "TB", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "TB.png") },
            { "TEN", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "TEN.png") },
            { "WAS", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo", "WAS.png") },
        };

        public static string GetLogoPath(string abbr)
        {
            return LogoPaths.TryGetValue(abbr, out var path) ? path : LogoPaths["default"];
        }
    }
}
