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
            { "ARI", "https://i.imgur.com/FeiV2pG.png" },
            { "ATL", "https://i.imgur.com/IVo1dYV.png" },
            { "BAL", "https://i.imgur.com/whCL1a7.png" },
            { "BUF", "https://i.imgur.com/R6aqEmv.png" },
            { "CAR", "https://i.imgur.com/6DKZNeY.png" },
            { "CHI", "https://i.imgur.com/JpRLsi6.png" },
            { "CIN", "https://i.imgur.com/CHylxYB.png" },
            { "CLE", "https://i.imgur.com/g4dvKUD.png" },
            { "DAL", "https://i.imgur.com/96WBkp0.png" },
            { "DEN", "https://i.imgur.com/WxEHaTN.png" },
            { "DET", "https://i.imgur.com/TQVeATC.png" },
            { "GB", "https://i.imgur.com/SJ2oto0.png" },
            { "HOU", "https://i.imgur.com/fofi40e.png" },
            { "IND", "https://i.imgur.com/sKUI0eb.png" },
            { "JAX", "https://i.imgur.com/pdhpchh.png" },
            { "KC", "https://i.imgur.com/77NyU5H.png" },
            { "LAC", "https://i.imgur.com/4WqBruj.png" },
            { "LAR", "https://i.imgur.com/fvtTMi1.png" },
            { "LV", "https://i.imgur.com/b5oOtR3.png" },
            { "MIA", "https://i.imgur.com/IicoEEn.png" },
            { "NE", "https://i.imgur.com/XeoQlW9.png" },
            { "NO", "https://i.imgur.com/oQEE4H1.png" },
            { "NYG", "https://i.imgur.com/MmNhzq2.png" },
            { "NYJ", "https://i.imgur.com/Pkwvb4C.png" },
            { "PHI", "https://i.imgur.com/aYh2AHU.png" },
            { "PIT", "https://i.imgur.com/SP3WvWe.png" },
            { "SEA", "https://i.imgur.com/7GwDP9y.png" },
            { "SF", "https://i.imgur.com/lIfgjm8.png" },
            { "TB", "https://i.imgur.com/OxR1wjp.png" },
            { "TEN", "https://i.imgur.com/dlg3jwK.png" },
            { "WAS", "https://i.imgur.com/a3q975O.png" },
        };

        public static string GetLogoPath(string abbr)
        {
            return LogoPaths.TryGetValue(abbr, out var path) ? path : LogoPaths["default"];
        }
    }
}
