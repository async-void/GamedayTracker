using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using GamedayTracker.Models;

namespace GamedayTracker.Extensions
{
    public static class TeamExtensions
    {
        #region TO TEAM LINK NAME
        public static string ToTeamLinkName(this string name)
        {
            var result = name switch
            {
                "buffalo" => "buffalo-bills",
                "ny giants" => "new-york-giants",
                "miami" => "miami-dolphins",
                "patriots" => "new-england-patriots",
                "ny jets" => "new-york-jets",
                "ravens" => "baltimore-ravens",
                "bengals" => "cincinnati-bengals",
                "browns" => "cleveland-browns",
                "pittsburgh" => "pittsburgh-steelers",
                "steelers" => "pittsburgh-steelers",
                "texans" => "houston-texans",
                "cults" => "indianapolis-colts",
                "jaguars" => "jacksonville-jaguars",
                "titans" => "tennessee-titans",
                "broncos" => "denver-broncos",
                "chiefs" => "kansas-city-chiefs",
                "raiders" => "las-vegas-raiders",
                "chargers" => "los-angeles-chargers",
                "cowboys" => "dallas-cowboys",
                "eagles" => "philadelphia-eagles",
                "washington" => "washington-commanders",
                "bears" => "chicago-bears",
                "lions" => "detroit-lions",
                "green bay" => "green-bay-packers",
                "vikings" => "minnesota-vikings",
                "falcons" => "atlanta-falcons",
                "carolina" => "carolina-panthers",
                "saints" => "new-orleans-saints",
                "tampa bay" => "tampa-bay-buccaneers",
                "arizona" => "arizona-cardinals",
                "rams" => "los-angeles-rams",
                "san fran" => "san-francisco-49ers",
                "seahawks" => "seattle-seahawks",
                _ => "UNKNOWN"
            };


            return result;
        }
        #endregion

        #region TO ABBREVIATION
        public static string ToAbbr(this string name)
        {
            var result = name switch
            {
                "Arizona" => "ARI",
                "Buffalo" => "BUF",
                "Miami" => "MIA",
                "New England" => "NE",
                "NY Jets" => "NYJ",
                "Baltimore" => "BAL",
                "Cincinnati" => "CIN",
                "Cleveland" => "CLE",
                "Pittsburgh" => "PIT",
                "Houston" => "HOU",
                "Indianapolis" => "IND",
                "Jacksonville" => "JAC",
                "Tennessee" => "TEN",
                "Denver" => "DEN",
                "Kansas City" => "KC",
                "Las Vegas" => "LAR",
                "LA Chargers" => "LAC",
                "Dallas" => "DAL",
                "NY Giants" => "NYG",
                "Philadelphia" => "PHI",
                "Washington" => "WAS",
                "Chicago" => "CHI",
                "Detroit" => "DET",
                "Green Bay" => "GB",
                "Minnesota" => "MIN",
                "Atlanta" => "ATL",
                "Carolina" => "CAR",
                "New Orleans" => "NO",
                "Tampa Bay" => "TB",
                "LA Rams" => "LAR",
                "San Francisco" => "SF",
                "Seattle" => "SEA",
                _ => "UNKNOWN"
            };

            return result;
        }
        #endregion

        #region TO DIVISION
        public static string ToDivision(this string name)
        {
            var result = name switch
            {
                "Buffalo" => "AFC EAST",
                "Miami" => "AFC EAST",
                "New England" => "AFC EAST",
                "NY Jets" => "AFC EAST",
                "Baltimore" => "AFC NORTH",
                "Cincinnati" => "AFC NORTH",
                "Cleveland" => "AFC NORTH",
                "Pittsburgh" => "AFC NORTH",
                "Houston" => "AFC SOUTH",
                "Indianapolis" => "AFC SOUTH",
                "Jacksonville" => "AFC SOUTH",
                "Tennessee" => "AFC SOUTH",
                "Denver" => "AFC WEST",
                "Kansas City" => "AFC WEST",
                "Las Vegas" => "AFC WEST",
                "LA Chargers" => "AFC WEST",
                "Dallas" => "NFC EAST",
                "NY Giants" => "NFC EAST",
                "Philadelphia" => "NFC EAST",
                "Washington" => "NFC EAST",
                "Chicago" => "NFC NORTH",
                "Detroit" => "NFC NORTH",
                "Green Bay" => "NFC NORTH",
                "Minnesota" => "NFC NORTH",
                "Atlanta" => "NFC SOUTH",
                "Carolina" => "NFC SOUTH",
                "New Orleans" => "NFC SOUTH",
                "Tampa Bay" => "NFC SOUTH",
                "Arizona" => "NFC WEST",
                "LA Rams" => "NFC WEST",
                "San Francisco" => "NFC WEST",
                "Seattle" => "NFC WEST",
                _ => "UNKNOWN"
            };

            return result;
        }
        #endregion

        #region TO TEAM FULL NAME

        public static string ToTeamFullName(this string name)
        {
            var jsonFile = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Json",
                "team_names.json"));

            var jsonNodes = JsonNode.Parse(jsonFile)!.GetValue<List<TeamJson>>();

            foreach (var jsonNode in jsonNodes.Where(jsonNode => jsonNode.FullName!.Equals(name)))
            {
                return jsonNode.FullName!;
            }

            return "Not Found";
        }
        #endregion
    }
}
