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
        { "bills", "Buffalo" },
        { "buffalo", "Buffalo" },

        { "dolphins", "Miami" },
        { "miami", "Miami" },
        { "fins", "Miami" },

        { "patriots", "New England" },
        { "new england", "New England" },
        { "pats", "New England" },

        { "jets", "NY Jets" },
        { "new york jets", "NY Jets" },

        // AFC North
        { "ravens", "Baltimore" },
        { "baltimore", "Baltimore" },

        { "bengals", "Cincinnati" },
        { "cincinnati", "Cincinnati" },
        { "cinci", "Cincinnati" },

        { "browns", "Cleveland" },
        { "cleveland", "Cleveland" },

        { "steelers", "Pittsburgh" },
        { "pittsburgh", "Pittsburgh" },
        { "steel curtain", "Pittsburgh" },

        // AFC South
        { "texans", "Houston" },
        { "houston", "Houston" },

        { "colts", "Indianapolis" },
        { "indianapolis", "Indianapolis" },
        { "indy", "Indianapolis" },

        { "jaguars", "Jacksonville" },
        { "jags", "Jacksonville" },
        { "jacksonville", "Jacksonville" },

        { "titans", "Tennessee" },
        { "tennessee", "Tennessee" },

        // AFC West
        { "broncos", "Denver" },
        { "denver", "Denver" },

        { "chiefs", "Kansas City" },
        { "kansas city", "Kansas City" },
        { "kc", "Kansas City" },

        { "raiders", "Las Vegas" },
        { "las vegas", "Las Vegas" },
        { "vegas", "Las Vegas" },
        { "oakland", "Las Vegas" }, // legacy

        { "chargers", "Los Angeles" },
        { "la chargers", "Los Angeles" },
        { "los angeles chargers", "Los Angeles" },
        { "san diego", "Los Angeles" }, // legacy

        // NFC East
        { "cowboys", "Dallas" },
        { "dallas", "Dallas" },
        { "america's team", "Dallas" },

        { "giants", "NY Giants" },
        { "new york giants", "NY Giants" },

        { "eagles", "Philadelphia" },
        { "philadelphia", "Philadelphia" },
        { "philly", "Philadelphia" },

        { "commanders", "Washington" },
        { "washington", "Washington" },
        { "redskins", "Washington" }, // legacy

        // NFC North
        { "bears", "Chicago" },
        { "chicago", "Chicago" },

        { "lions", "Detroit" },
        { "detroit", "Detroit" },

        { "packers", "Green Bay" },
        { "green bay", "Green Bay" },
        { "cheeseheads", "Green Bay" },

        { "vikings", "Minnesota" },
        { "minnesota", "Minnesota" },
        { "skol", "Minnesota" },

        // NFC South
        { "falcons", "Atlanta" },
        { "atlanta", "Atlanta" },

        { "panthers", "Carolina" },
        { "carolina", "Carolina" },

        { "saints", "New Orleans" },
        { "new orleans", "New Orleans" },
        { "who dat", "New Orleans" },

        { "buccaneers", "Tampa Bay" },
        { "bucs", "Tampa Bay" },
        { "tampa", "Tampa Bay" },
        { "tampa bay", "Tampa Bay" },

        // NFC West
        { "cardinals", "Arizona" },
        { "arizona", "Arizona" },
        { "cards", "Arizona" },

        { "rams", "Los Angeles" },
        { "la rams", "Los Angeles" },
        { "los angeles rams", "Los Angeles" },
        { "st louis", "Los Angeles" }, // legacy

        { "49ers", "San Francisco" },
        { "niners", "San Francisco" },
        { "san francisco", "San Francisco" },

        { "seahawks", "Seattle" },
        { "seattle", "Seattle" },
        { "hawks", "Seattle" }
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
