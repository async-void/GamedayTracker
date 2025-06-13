using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                "Buffalo" => "buffalo-bills",
                "NY Giants" => "new-york-giants",
                "Miami" => "miami-dolphins",
                "New England" => "new-england-patriots",
                "NY Jets" => "new-york-jets",
                "Baltimore" => "baltimore-ravens",
                "Cincinnati" => "cincinnati-bengals",
                "Cleveland" => "cleveland-browns",
                "Pittsburgh" => "pittsburgh-steelers",
                "Houston" => "houston-texans",
                "Indianapolis" => "indianapolis-colts",
                "Jacksonville" => "jacksonville-jaguars",
                "Tennessee" => "tennessee-titans",
                "Denver" => "denver-broncos",
                "Kansas City" => "kansas-city-chiefs",
                "Las Vegas" => "las-vegas-raiders",
                "Los Angeles" => "los-angeles-chargers",
                "Dallas" => "dallas-cowboys",
                "Philadelphia" => "philadelphia-eagles",
                "Washington" => "washington-commanders",
                "Chicago" => "chicago-bears",
                "Detroit" => "detroit-lions",
                "Green Bay" => "green-bay-packers",
                "Minnesota" => "minnesota-vikings",
                "Atlanta" => "atlanta-falcons",
                "Carolina" => "carolina-panthers",
                "New Orleans" => "new-orleans-saints",
                "Tampa Bay" => "tampa-bay-buccaneers",
                "Arizona" => "arizona-cardinals",
                "LA Rams" => "los-angeles-rams",
                "San Francisco" => "san-francisco-49ers",
                "Seattle" => "seattle-seahawks",
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
                "Louis" => "LAR",
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
                "Las Vegas" => "LV",
                "Oakland" => "LV",
                "LA Chargers" => "LAC",
                "San Diego" => "LAC",
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
                "Oakland" => "AFC WEST",
                "LA Chargers" => "AFC WEST",
                "San Diego" => "AFC WEST",
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
                " Louis" => "NFC WEST",
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
            var result = name switch
            {
                "Buffalo" => "Buffalo Bills",
                "Miami"=> "Miami Dolphins",
                "NY Jets" =>  "New York Jets",
                "New England" => "New England Patriots",
                "Baltimore" => "Baltimore Ravens",
                "Pittsburgh" => "Pittsburgh Steelers",
                "Cincinnati" => "Cincinnati Bengals",
                "Cleveland" =>  "Cleveland Browns",
                "Houston" => "Houston Texans",
                "Indianapolis" => "Indianapolis Colts",
                "Jacksonville" => "Jacksonville Jaguars",
                "Tennessee" => "Tennessee Titans",
                "Kansas City" => "Kansas City Chiefs",
                "LA Chargers" => "Los Angeles Chargers",
                "Denver" => "Denver Broncos",
                "Las Vegas" => "Las Vegas Raiders",
                "Philadelphia" => "Philadelphia Eagles",
                "Washington" => "Washington Commanders",
                "Dallas" => "Dallas Cowboys",
                "NY Giants" => "NY Giants",
                "Detroit" => "Detroit Lions",
                "Minnesota" => "Minnesota Vikings",
                "Green Bay" => "Green Bay Packers",
                "Chicago" => "Chicago Bears",
                "Tampa Bay" => "Tampa Bay Buccaneers",
                "Atlanta" => "Atlanta Falcons",
                "Carolina" => "Carolina Panthers",
                "New Orleans" => "New Orleans Saints",
                "LA Rams" => "Los Angeles Rams",
                "Seattle" => "Seattle Seahawks",
                "Arizona" => "Arizona Cardinals",
                "San Francisco" => "San Francisco 49ers",
                _ => name
            };

            return result;
        }
        #endregion

        #region TO TEAM SHORT NAME

        public static string ToShortName(this string name)
        {
            var result = name switch
            {
                "Buffalo Bills" => "Buffalo",
                "Miami Dolphins" => "Miami",
                "NY Jets" => "NY Jets",
                "New England Patriots" => "New England",
                "Baltimore Ravens" => "Baltimore",
                "Pittsburgh Steelers" => "Pittsburgh",
                "Cincinnati Bengals" => "Cincinnati",
                "Cleveland Browns" => "Cleveland",
                "Houston Texans" => "Houston",
                "Indianapolis Colts" => "Indianapolis",
                "Jacksonville Jaguars" => "Jacksonville",
                "Tennessee Titans" => "Tennessee",
                "Kansas City Chiefs" => "Kansas City",
                "Los Angeles Chargers" => "LA Chargers",
                "Denver Broncos" => "Denver",
                "Las Vegas Raiders" => "Las Vegas",
                "Philadelphia Eagles" => "Philadelphia",
                "Washington Commanders" => "Washington",
                "Dallas Cowboys" => "Dallas",
                "NY Giants" => "NY Giants",
                "Detroit Lions" => "Detroit",
                "Minnesota Vikings" => "Minnesota",
                "Green Bay Packers" => "Green Bay",
                "Chicago Bears" => "Chicago",
                "Tampa Bay Buccaneers" => "Tampa Bay",
                "Atlanta Falcons" => "Atlanta",
                "Carolina Panthers" => "Carolina",
                "New Orleans Saints" => "New Orleans",
                "Los Angeles Rams" => "LA Rams",
                "Seattle Seahawks" => "Seattle",
                "Arizona Cardinals" => "Arizona",
                "San Francisco 49ers" => "San Francisco",
                _ => name
            };

            return result;
        }
        #endregion
    }
}
