using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public static class TeamAliasMap
    {
        public static readonly Dictionary<string, string> Map = BuildMap();

        private static Dictionary<string, string> BuildMap()
        {
            var teams = new[]
            {
            new { Code = "ARI", Aliases = new[] { "arizona", "arizona cardinals", "cards", "cardinals", "redbirds" } },
            new { Code = "LAR", Aliases = new[] { "la rams", "los angeles rams", "rams", "st louis", "louis", "st louis rams" } },
            new { Code = "SF",  Aliases = new[] { "san francisco", "san francisco 49ers", "49ers", "niners", "whiners", "sf" } },
            new { Code = "SEA", Aliases = new[] { "seattle", "seattle seahawks", "seahawks", "hawks" } },

            new { Code = "BUF", Aliases = new[] { "buffalo", "buffalo bills", "bills", "buf" } },
            new { Code = "MIA", Aliases = new[] { "miami", "miami dolphins", "dolphins", "the fish", "fins" } },
            new { Code = "NE",  Aliases = new[] { "new england", "new england patriots", "patriots", "pats" } },
            new { Code = "NYJ", Aliases = new[] { "ny jets", "jets", "new york jets" } },

            new { Code = "BAL", Aliases = new[] { "baltimore", "baltimore ravens", "ravens", "ratbirds" } },
            new { Code = "CIN", Aliases = new[] { "cincinnati", "cincinnati bengals", "bengals", "bungles" } },
            new { Code = "CLE", Aliases = new[] { "cleveland", "cleveland browns", "browns" } },
            new { Code = "PIT", Aliases = new[] { "pittsburgh", "pittsburgh steelers", "steelers", "pit" } },

            new { Code = "HOU", Aliases = new[] { "houston", "houston texans", "texans" } },
            new { Code = "IND", Aliases = new[] { "indianapolis", "indianapolis colts", "colts" } },
            new { Code = "JAX", Aliases = new[] { "jacksonville", "jacksonville jaguars", "jaguars", "jags" } },
            new { Code = "TEN", Aliases = new[] { "tennessee", "tennessee titans", "titans" } },

            new { Code = "DEN", Aliases = new[] { "denver", "denver broncos", "broncos", "donkeys" } },
            new { Code = "KC",  Aliases = new[] { "kansas city", "kansas city chiefs", "chiefs" } },
            new { Code = "LV",  Aliases = new[] { "las vegas", "las vegas raiders", "oakland", "oakland raiders", "raiders", "lv" } },
            new { Code = "LAC", Aliases = new[] { "la chargers", "los angeles chargers", "chargers", "san diego", "san diego chargers", "bolts" } },

            new { Code = "DAL", Aliases = new[] { "dallas", "dallas cowboys", "cowboys", "boys" } },
            new { Code = "NYG", Aliases = new[] { "ny giants", "new york giants", "giants" } },
            new { Code = "PHI", Aliases = new[] { "philadelphia", "philadelphia eagles", "eagles", "iggles" } },
            new { Code = "WSH", Aliases = new[] { "washington", "washington commanders", "commanders", "skins", "redskins" } },

            new { Code = "CHI", Aliases = new[] { "chicago", "chicago bears", "bears", "da bears" } },
            new { Code = "DET", Aliases = new[] { "detroit", "detroit lions", "lions" } },
            new { Code = "GB",  Aliases = new[] { "green bay", "green bay packers", "packers", "cheeseheads" } },
            new { Code = "MIN", Aliases = new[] { "minnesota", "minnesota vikings", "vikings" } },

            new { Code = "ATL", Aliases = new[] { "atlanta", "atlanta falcons", "falcons", "dirty birds" } },
            new { Code = "CAR", Aliases = new[] { "carolina", "carolina panthers", "panthers" } },
            new { Code = "NO",  Aliases = new[] { "new orleans", "new orleans saints", "saints", "who dat", "aints" } },
            new { Code = "TB",  Aliases = new[] { "tampa bay", "tampa bay buccaneers", "buccaneers", "bucs" } },
        };

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var t in teams)
                foreach (var alias in t.Aliases)
                    map[Normalize(alias)] = t.Code;

            return map;
        }

        private static string Normalize(string input)
        {
            return input
                .Trim()
                .ToLowerInvariant()
                .Replace(".", "")
                .Replace(",", "")
                .Replace("-", " ")
                .Replace("  ", " ");
        }
    }

}
