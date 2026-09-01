using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public static class NflTeamAliasResolver
    {
        public static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
        {
            // AFC EAST
            ["bills"] = "bills",
            ["buffalo"] = "bills",
            ["buffalo bills"] = "bills",

            ["dolphins"] = "dolphins",
            ["miami"] = "dolphins",
            ["miami dolphins"] = "dolphins",
            ["fins"] = "dolphins",

            ["patriots"] = "patriots",
            ["new england"] = "patriots",
            ["new england patriots"] = "patriots",
            ["pats"] = "patriots",

            ["jets"] = "jets",
            ["new york jets"] = "jets",
            ["nyj"] = "jets",
            ["jets"] = "jets",

            // AFC NORTH
            ["ravens"] = "ravens",
            ["baltimore"] = "ravens",
            ["baltimore ravens"] = "ravens",

            ["bengals"] = "bengals",
            ["cincinnati"] = "bengals",
            ["cincinnati bengals"] = "bengals",
            ["cin"] = "bengals",

            ["browns"] = "browns",
            ["cleveland"] = "browns",
            ["cleveland browns"] = "browns",

            ["steelers"] = "steelers",
            ["pittsburgh"] = "steelers",
            ["pittsburgh steelers"] = "steelers",
            ["pit"] = "steelers",
            ["steelers"] = "steelers",

            // AFC SOUTH
            ["texans"] = "texans",
            ["houston"] = "texans",
            ["houston texans"] = "texans",

            ["colts"] = "colts",
            ["indianapolis"] = "colts",
            ["indianapolis colts"] = "colts",

            ["jaguars"] = "jaguars",
            ["jacksonville"] = "jaguars",
            ["jacksonville jaguars"] = "jaguars",
            ["jags"] = "jaguars",

            ["titans"] = "titans",
            ["tennessee"] = "titans",
            ["tennessee titans"] = "titans",

            // AFC WEST
            ["broncos"] = "broncos",
            ["denver"] = "broncos",
            ["denver broncos"] = "broncos",

            ["chiefs"] = "chiefs",
            ["kansas city"] = "chiefs",
            ["kansas city chiefs"] = "chiefs",
            ["kc"] = "chiefs",

            ["raiders"] = "raiders",
            ["las vegas"] = "raiders",
            ["las vegas raiders"] = "raiders",
            ["oakland raiders"] = "raiders",
            ["oakland"] = "raiders",

            ["chargers"] = "chargers",
            ["los angeles chargers"] = "chargers",
            ["la chargers"] = "chargers",
            ["san diego chargers"] = "chargers",
            ["sd chargers"] = "chargers",

            // NFC EAST
            ["cowboys"] = "cowboys",
            ["dallas"] = "cowboys",
            ["dallas cowboys"] = "cowboys",
            ["america's team"] = "cowboys",

            ["giants"] = "giants",
            ["new york giants"] = "giants",
            ["nyg"] = "giants",

            ["eagles"] = "eagles",
            ["philadelphia"] = "eagles",
            ["philadelphia eagles"] = "eagles",

            ["commanders"] = "commanders",
            ["washington"] = "commanders",
            ["washington commanders"] = "commanders",
            ["redskins"] = "commanders",
            ["washington football team"] = "commanders",

            // NFC NORTH
            ["bears"] = "bears",
            ["chicago"] = "bears",
            ["chicago bears"] = "bears",
            ["da bears"] = "bears",

            ["lions"] = "lions",
            ["detroit"] = "lions",
            ["detroit lions"] = "lions",

            ["packers"] = "packers",
            ["green bay"] = "packers",
            ["green bay packers"] = "packers",
            ["gb"] = "packers",

            ["vikings"] = "vikings",
            ["minnesota"] = "vikings",
            ["minnesota vikings"] = "vikings",
            ["skol"] = "vikings",

            // NFC SOUTH
            ["falcons"] = "falcons",
            ["atlanta"] = "falcons",
            ["atlanta falcons"] = "falcons",

            ["panthers"] = "panthers",
            ["carolina"] = "panthers",
            ["carolina panthers"] = "panthers",

            ["saints"] = "saints",
            ["new orleans"] = "saints",
            ["new orleans saints"] = "saints",
            ["who dat"] = "saints",

            ["buccaneers"] = "buccaneers",
            ["tampa bay"] = "buccaneers",
            ["tampa bay buccaneers"] = "buccaneers",
            ["bucs"] = "buccaneers",

            // NFC WEST
            ["cardinals"] = "cardinals",
            ["arizona"] = "cardinals",
            ["arizona cardinals"] = "cardinals",

            ["rams"] = "rams",
            ["los angeles rams"] = "rams",
            ["la rams"] = "rams",
            ["st louis rams"] = "rams",

            ["49ers"] = "49ers",
            ["san francisco"] = "49ers",
            ["san francisco 49ers"] = "49ers",
            ["niners"] = "49ers",
            ["sf"] = "49ers",

            ["seahawks"] = "seahawks",
            ["seattle"] = "seahawks",
            ["seattle seahawks"] = "seahawks",
            ["hawks"] = "seahawks",
        };

        public static bool TryResolveTeamToShortName(this string input, out string canonical)
            => Aliases.TryGetValue(input.Trim(), out canonical);

        public static string ResolveTeamToShortName(this string input)
            => Aliases.TryGetValue(input.Trim(), out var canonical)
                ? canonical : throw new KeyNotFoundException($"Unknown NFL team: '{input}'.");
    }

}
