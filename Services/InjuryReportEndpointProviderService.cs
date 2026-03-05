using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public static class InjuryReportEndpointProviderService
    {
        private static readonly Dictionary<string, int> TeamIds = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Arizona Cardinals"] = 22,
            ["Atlanta Falcons"] = 1,
            ["Baltimore Ravens"] = 33,
            ["Buffalo Bills"] = 2,
            ["Carolina Panthers"] = 29,
            ["Chicago Bears"] = 3,
            ["Cincinnati Bengals"] = 4,
            ["Cleveland Browns"] = 5,
            ["Dallas Cowboys"] = 6,
            ["Denver Broncos"] = 7,
            ["Detroit Lions"] = 8,
            ["Green Bay Packers"] = 9,
            ["Houston Texans"] = 34,
            ["Indianapolis Colts"] = 11,
            ["Jacksonville Jaguars"] = 30,
            ["Kansas City Chiefs"] = 12,
            ["Las Vegas Raiders"] = 13,
            ["Los Angeles Chargers"] = 24,
            ["Los Angeles Rams"] = 14,
            ["Miami Dolphins"] = 15,
            ["Minnesota Vikings"] = 16,
            ["New England Patriots"] = 17,
            ["New Orleans Saints"] = 18,
            ["New York Giants"] = 19,
            ["New York Jets"] = 20,
            ["Philadelphia Eagles"] = 21,
            ["Pittsburgh Steelers"] = 23,
            ["San Francisco 49ers"] = 25,
            ["Seattle Seahawks"] = 26,
            ["Tampa Bay Buccaneers"] = 27,
            ["Tennessee Titans"] = 10,
            ["Washington Commanders"] = 28

        };

        public static string GetTeamInjuryReport(string teamName)
        {
            if (string.IsNullOrWhiteSpace(teamName))
                throw new ArgumentException("Team name cannot be empty.", nameof(teamName));

            if (!TeamIds.TryGetValue(teamName.Trim(), out var teamId))
                throw new KeyNotFoundException($"Unknown team name '{teamName}'. Add it to the TeamIds dictionary.");

            return $"https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/{teamId}/injuries";
        }


    }
}
