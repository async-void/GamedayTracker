using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public static class NflTeamMatcher
    {
        public static readonly Dictionary<string, string> TeamAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            // AFC East
            { "bills", "Buffalo Bills" },
            { "buffalo", "Buffalo Bills" },
            { "buf", "Buffalo Bills" },

            { "dolphins", "Miami Dolphins" },
            { "miami", "Miami Dolphins" },
            { "fins", "Miami Dolphins" },
            { "mia", "Miami Dolphins" },

            { "patriots", "New England Patriots" },
            { "new england", "New England Patriots" },
            { "pats", "New England Patriots" },
            { "ne", "New England Patriots" },

            { "jets", "New York Jet" },
            { "new york jets", "New York Jet" },
            { "nyj", "New York Jet" },

            // AFC North
            { "ravens", "Baltimore Ravens" },
            { "baltimore", "Baltimore Ravens" },
            { "bal", "Baltimore Ravens" },

            { "bengals", "Cincinnati Bengals" },
            { "cincinnati", "Cincinnati Bengals" },
            { "cinci", "Cincinnati Bengals" },
            { "cin", "Cincinnati Bengals" },

            { "browns", "Cleveland Browns" },
            { "cleveland", "Cleveland Browns" },
            { "cle", "Cleveland Browns" },

            { "steelers", "Pittsburgh Steelers" },
            { "pittsburgh", "Pittsburgh Steelers" },
            { "steel curtain", "Pittsburgh Steelers" },
            { "pit", "Pittsburgh Steelers" },

            // AFC South
            { "texans", "Houston Texans" },
            { "houston", "Houston Texans" },
            { "hou", "Houston Texans" },

            { "colts", "Indianapolis Colts" },
            { "indianapolis", "Indianapolis Colts" },
            { "indy", "Indianapolis Colts" },
            { "ind", "Indianapolis Colts" },

            { "jaguars", "Jacksonville Jaguars" },
            { "jags", "Jacksonville Jaguars" },
            { "jacksonville", "Jacksonville Jaguars" },
            { "jac", "Jacksonville Jaguars" },

            { "titans", "Tennessee Titans" },
            { "tennessee", "Tennessee Titans" },
            { "ten", "Tennessee Titans" },

            // AFC West
            { "broncos", "Denver Broncos" },
            { "denver", "Denver Broncos" },
            { "den", "Denver Broncos" },

            { "chiefs", "Kansas City Chiefs" },
            { "kansas city", "Kansas City Chiefs" },
            { "kc", "Kansas City Chiefs" },

            { "raiders", "Las Vegas Raiders" },
            { "las vegas", "Las Vegas Raiders" },
            { "vegas", "Las Vegas Raiders" },
            { "lav", "Las Vegas Raiders" },
            { "oakland", "Las Vegas Raiders" }, // legacy

            { "chargers", "Los Angeles Chargers" },
            { "la chargers", "Los Angeles Chargers" },
            { "los angeles chargers", "Los Angeles Chargers" },
            { "lac", "Los Angeles Chargers" },
            { "san diego", "Los Angeles Chargers" }, // legacy

            // NFC East
            { "cowboys", "Dallas Cowboys" },
            { "dallas", "Dallas Cowboys" },
            { "america's team", "Dallas Cowboys" },
            { "dal", "Dallas Cowboys" },

            { "giants", "NY Giants" },
            { "new york giants", "NY Giants" },
            { "nyg", "NY Giants" },

            { "eagles", "Philadelphia Eagles" },
            { "philadelphia", "Philadelphia Eagles" },
            { "philly", "Philadelphia Eagles" },
            { "phi", "Philadelphia Eagles" },

            { "commanders", "Washington Commanders" },
            { "washington", "Washington Commanders" },
            { "was", "Washington Commanders" },
            { "redskins", "Washington Commanders" }, // legacy

            // NFC North
            { "bears", "Chicago Bears" },
            { "chicago", "Chicago Bears" },
            { "da bears", "Chicago Bears" },
            { "chi", "Chicago Bears" },

            { "lions", "Detroit Lions" },
            { "detroit", "Detroit Lions" },
            { "det", "Detroit Lions" },

            { "packers", "Green Bay Packers" },
            { "green bay", "Green Bay Packers" },
            { "cheeseheads", "Green Bay Packers" },
            { "gb", "Green Bay Packers" },

            { "vikings", "Minnesota Vikings" },
            { "minnesota", "Minnesota Vikings" },
            { "skol", "Minnesota Vikings" },
            { "min", "Minnesota Vikings" },

            // NFC South
            { "falcons", "Atlanta Falcons" },
            { "atlanta", "Atlanta Falcons" },
            { "atl", "Atlanta Falcons" },

            { "panthers", "Carolina Panthers" },
            { "carolina", "Carolina Panthers" },
            { "car", "Carolina Panthers" },

            { "saints", "New Orleans Saints" },
            { "new orleans", "New Orleans Saints" },
            { "who dat", "New Orleans Saints" },
            { "no", "New Orleans Saints" },

            { "buccaneers", "Tampa Bay Buccaneers" },
            { "bucs", "Tampa Bay Buccaneers" },
            { "tampa", "Tampa Bay Buccaneers" },
            { "tampa bay", "Tampa Bay Buccaneers" },
            { "tb", "Tampa Bay Buccaneers" },

            // NFC West
            { "cardinals", "Arizona Cardinals" },
            { "arizona", "Arizona Cardinals" },
            { "cards", "Arizona Cardinals" },
            { "ari", "Arizona Cardinals" },

            { "rams", "Los Angeles Rams" },
            { "la rams", "Los Angeles Rams" },
            { "los angeles rams", "Los Angeles Rams" },
            { "lar", "Los Angeles Rams" },
            { "st louis", "Los Angeles Rams" }, // legacy

            { "49ers", "San Francisco 49ers" },
            { "niners", "San Francisco 49ers" },
            { "san francisco", "San Francisco 49ers" },
            { "sf", "San Francisco 49ers" },

            { "seahawks", "Seattle Seahawks" },
            { "seattle", "Seattle Seahawks" },
            { "hawks", "Seattle Seahawks" },
            { "sea", "Seattle Seahawks" }
        };

        public static string? MatchTeam(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return null;

            var normalized = userInput.Trim().ToLowerInvariant();
            if (TeamAliases.TryGetValue(normalized, out var exactMatch))
                return exactMatch;

            var bestMatch = Process.ExtractOne(normalized, TeamAliases.Keys);
            if (bestMatch != null && bestMatch.Score >= 80) // Adjust threshold as needed
                return TeamAliases[bestMatch.Value];

            return null;
        }
    }
}
