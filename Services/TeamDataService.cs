using System.Diagnostics;
using System.Globalization;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Services
{
    public class TeamDataService(IJsonDataService jsonDataService) : ITeamData
    {
        #region AFC SELECT OPTIONS
        public Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForAfc()
        {
            var options = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption("Buffalo Bills", "Buffalo Bills"),
                new DiscordSelectComponentOption("Miami Dolphins", "Miami Dolphins"),
                new DiscordSelectComponentOption("New England Patriots", "New England Patriots"),
                new DiscordSelectComponentOption("New York Jets", "New York Jets"),
                new DiscordSelectComponentOption("Baltimore Ravens", "Baltimore Ravens"),
                new DiscordSelectComponentOption("Cincinnati Bengals", "Cincinnati Bengals"),
                new DiscordSelectComponentOption("Cleveland Browns", "Cleveland Browns"),
                new DiscordSelectComponentOption("Pittsburgh Steelers", "Pittsburgh Steelers"),
                new DiscordSelectComponentOption("Houston Texans", "Houston Texans"),
                new DiscordSelectComponentOption("Indianapolis Colts", "Indianapolis Colts"),
                new DiscordSelectComponentOption("Jacksonville Jaguars", "Jacksonville Jaguars"),
                new DiscordSelectComponentOption("Tennessee Titans", "Tennessee Titans"),
                new DiscordSelectComponentOption("Denver Broncos", "Denver Broncos"),
                new DiscordSelectComponentOption("Kansas City Chiefs", "Kansas City Chiefs"),
                new DiscordSelectComponentOption("Las Vegas Raiders", "Las Vegas Raiders"),
                new DiscordSelectComponentOption("Los Angeles Chargers", "Los Angeles Chargers"),
            };

            return Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>>.Ok(options);
        }
        #endregion

        #region NFC SELECT OPTIONS
        public Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForNfc()
        {
            var nfcOptions = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption("Dallas Cowboys", "Dallas Cowboys"),
                new DiscordSelectComponentOption("New York Giants", "New York Giants"),
                new DiscordSelectComponentOption("Philadelphia Eagles", "Philadelphia Eagles"),
                new DiscordSelectComponentOption("Washington Commanders", "Washington Commanders"),
                new DiscordSelectComponentOption("Chicago Bears", "Chicago Bears"),
                new DiscordSelectComponentOption("Detroit Lions", "Detroit Lions"),
                new DiscordSelectComponentOption("Green Bay Packers", "Green Bay Packers"),
                new DiscordSelectComponentOption("Minnesota Vikings", "Minnesota Vikings"),
                new DiscordSelectComponentOption("Atlanta Falcons", "Atlanta Falcons"),
                new DiscordSelectComponentOption("Carolina Panthers", "Carolina Panthers"),
                new DiscordSelectComponentOption("New Orleans Saints", "New Orleans Saints"),
                new DiscordSelectComponentOption("Tampa Bay Buccaneers", "Tampa Bay Buccaneers"),
                new DiscordSelectComponentOption("Arizona Cardinals", "Arizona Cardinals"),
                new DiscordSelectComponentOption("Los Angeles Rams", "Los Angeles Rams"),
                new DiscordSelectComponentOption("San Francisco 49ers", "San Francisco 49ers"),
                new DiscordSelectComponentOption("Seattle Seahawks", "Seattle Seahawks"),
            };
            return Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>>.Ok(nfcOptions);
        }
        #endregion

        #region USER PICKS SELECT OPTIONS

        public Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForUserPicks()
        {
            var options = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption("Buffalo Bills", "Buffalo Bills"),
                new DiscordSelectComponentOption("Miami Dolphins", "Miami Dolphins"),
                new DiscordSelectComponentOption("New England Patriots", "New England Patriots"),
                new DiscordSelectComponentOption("New York Jets", "New York Jets"),
                new DiscordSelectComponentOption("Baltimore Ravens", "Baltimore Ravens"),
                new DiscordSelectComponentOption("Cincinnati Bengals", "Cincinnati Bengals"),
                new DiscordSelectComponentOption("Cleveland Browns", "Cleveland Browns"),
                new DiscordSelectComponentOption("Pittsburgh Steelers", "Pittsburgh Steelers"),
                new DiscordSelectComponentOption("Houston Texans", "Houston Texans"),
                new DiscordSelectComponentOption("Indianapolis Colts", "Indianapolis Colts"),
                new DiscordSelectComponentOption("Jacksonville Jaguars", "Jacksonville Jaguars"),
                new DiscordSelectComponentOption("Tennessee Titans", "Tennessee Titans"),
                new DiscordSelectComponentOption("Denver Broncos", "Denver Broncos"),
                new DiscordSelectComponentOption("Kansas City Chiefs", "Kansas City Chiefs"),
                new DiscordSelectComponentOption("Las Vegas Raiders", "Las Vegas Raiders"),
                new DiscordSelectComponentOption("Los Angeles Chargers", "Los Angeles Chargers"),
                new DiscordSelectComponentOption("Dallas Cowboys", "Dallas Cowboys"),
                new DiscordSelectComponentOption("New York Giants", "New York Giants"),
                new DiscordSelectComponentOption("Philadelphia Eagles", "Philadelphia Eagles"),
                new DiscordSelectComponentOption("Washington Commanders", "Washington Commanders"),
                new DiscordSelectComponentOption("Chicago Bears", "Chicago Bears"),
                new DiscordSelectComponentOption("Detroit Lions", "Detroit Lions"),
                new DiscordSelectComponentOption("Green Bay Packers", "Green Bay Packers"),
                new DiscordSelectComponentOption("Minnesota Vikings", "Minnesota Vikings"),
                new DiscordSelectComponentOption("Atlanta Falcons", "Atlanta Falcons"),
                new DiscordSelectComponentOption("Carolina Panthers", "Carolina Panthers"),
                new DiscordSelectComponentOption("New Orleans Saints", "New Orleans Saints"),
                new DiscordSelectComponentOption("Tampa Bay Buccaneers", "Tampa Bay Buccaneers"),
                new DiscordSelectComponentOption("Arizona Cardinals", "Arizona Cardinals"),
                new DiscordSelectComponentOption("Los Angeles Rams", "Los Angeles Rams"),
                new DiscordSelectComponentOption("San Francisco 49ers", "San Francisco 49ers"),
                new DiscordSelectComponentOption("Seattle Seahawks", "Seattle Seahawks"),
            };

            return Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>>.Ok(options);
        }

        #endregion

        #region GET DRAFT RESULT FOR TEAM
        public async Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultForTeamAsync(int year, string tName)
        {
            var draftList = await jsonDataService.GetDraftFromJsonAsync(year, tName);
            if (draftList.IsOk)
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(draftList.Value);

            var entityList = new List<DraftEntity>();
           
            for (var i = 1; i < 8; i++)
            {
                var link = $"https://www.footballdb.com/draft/draft.html?lg=NFL&yr={year}&rnd={i}";
                var web = new HtmlWeb();
                var doc = web.Load(link);

                var nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                if (nodes is null) continue;
                var nodeCount = nodes.Count;
                for (var j = 0; j < nodeCount; j++)
                {
                    var curNode = nodes[j];
                    if (!curNode.HasChildNodes) continue;
                    if (curNode.ChildNodes.Count != 7) continue;

                    var round = curNode.ChildNodes[0].InnerText.Split(" ")[0];
                    var pick = curNode.ChildNodes[1].InnerText;
                    var teamName = curNode.ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText;
                    var playerName = curNode.ChildNodes[3].InnerText;
                    var pos = curNode.ChildNodes[4].InnerText;
                    var college = curNode.ChildNodes[5].InnerText;

                    var de = new DraftEntity()
                    {
                        Season = year,
                        College = college,
                        PickPosition = pick,
                        PlayerName = playerName,
                        Pos = pos,
                        Round = round,
                        TeamName = teamName
                    };
                    entityList.Add(de);
                }
            }

            await jsonDataService.WriteDraftToJsonAsync(entityList, year);

            if (entityList.Count == 0)
            {
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
                {
                    ErrorMessage = "entity list was empty.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            var chosen = entityList.Where(x => x.TeamName.Equals(tName.ToTeamFullName())).ToList();
            return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(chosen);
        }
        #endregion

        #region GET DRAFT RESULTS
        public async Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultsAsync(string teamName, int season)
        {
            var draft = await jsonDataService.GetDraftFromJsonAsync(season, teamName);
            if (draft.IsOk)
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(draft.Value);

            var entityList = new List<DraftEntity>();
            
            for (var i = 1; i < 8; i++)
            {
                var link = $"https://www.footballdb.com/draft/draft.html?lg=NFL&yr={season}&rnd={i}";
                var web = new HtmlWeb();
                var doc = web.Load(link);

                var nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                if (nodes is null) continue;
                var nodeCount = nodes.Count;
                for (var j = 0; j < nodeCount; j++)
                {
                    var curNode = nodes[j];
                    if (!curNode.HasChildNodes) continue;
                    if (curNode.ChildNodes.Count !>= 7) continue;

                    var round = curNode.ChildNodes[0].InnerText.Split(" ")[0];
                    var pick = curNode.ChildNodes[1].InnerText;
                    var name = curNode.ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText;
                    var playerName = curNode.ChildNodes[3].InnerText;
                    var pos = curNode.ChildNodes[4].InnerText;
                    var college = curNode.ChildNodes[5].InnerText;

                    var de = new DraftEntity()
                    {
                        Season = season,
                        College = college,
                        PickPosition = pick,
                        PlayerName = playerName,
                        Pos = pos,
                        Round = round,
                        TeamName = teamName
                    };
                    entityList.Add(de);
                }
            }
            await jsonDataService.WriteDraftToJsonAsync(entityList, season);

            if (entityList.Count == 0)
            {
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
                {
                    ErrorMessage = "entity list was empty.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            
            return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(entityList);
        }
        #endregion

        #region INT TO TEAMNAME
        public Result<string, SystemError<TeamDataService>> GetTeamNameFromInt(int input)
        {
            var result = input switch
            {
                0 => "Buffalo Bills",
                _ => "Unknown"
            };
            return Result<string, SystemError<TeamDataService>>.Ok(result);
        }
        #endregion

        #region IS VALID TEAM NAME
        public bool IsValidTeamName(string name)
        {
            var teams = new List<string>
            {
                "arizona", 
                "atlanta", 
                "baltimore",
                "buffalo", 
                "carolina",
                "chicago", 
                "cincinnati", 
                "cleveland",
                "dallas", 
                "denver", 
                "detroit",
                "green bay",
                "houston",
                "indianapolis",
                "jacksonville", 
                "kansas city", 
                "las vegas", 
                "la chargers",
                "la rams", 
                "miami", 
                "minnesota", 
                "new england", 
                "new Orleans",
                "ny giants", 
                "ny jets", 
                "philadelphia",
                "pittsburgh", 
                "san francisco", 
                "seattle",
                "tampa bay",
                "tennessee", 
                "washington"
            };
            return teams.Contains(name);
        }
        #endregion

        #region GET ALL TEAM STATS

        public async Task<Result<List<TeamStats>, SystemError<TeamDataService>>> GetStatsAsync(int choice, int season)
        {
            await using var db = new AppDbContextFactory().CreateDbContext();
            var lineType = choice == 0 ? LineType.Offense : LineType.Defense;
            var statList = db.TeamStats.Where(x => x.Season == season && x.LineType.Equals(lineType)).ToList();
            if (statList.Count > 0)
            {
                return Result<List<TeamStats>, SystemError<TeamDataService>>.Ok(statList);
            }

            var link = "";
            var web = new HtmlWeb();

            link = choice switch
            {
                0 =>
                    $"https://www.footballdb.com/statistics/nfl/team-stats/offense-totals/{season}/regular-season?sort=ydsgm",
                1 =>
                    $"https://www.footballdb.com/statistics/nfl/team-stats/defense-totals/{season}/regular-season?sort=ydsgm",
                _ => link
            };

            var doc = web.Load(link);
            var stats = new List<TeamStats>();
            var nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");

            if (nodes is null) return Result<List<TeamStats>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
            {
                ErrorMessage = "No nodes found",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });

            for (var i = 0; i < nodes.Count; i++)
            {
                var curNode = nodes[i];
                if (!curNode.HasChildNodes) continue;

                var name = curNode.ChildNodes[0].ChildNodes[1].InnerText;
                var gamesPlayed = curNode.ChildNodes[1].InnerText.ToInt();
                var totalPoints = curNode.ChildNodes[2].InnerText.ToInt();
                var pointsPerGame = curNode.ChildNodes[3].InnerText.Replace(",", string.Empty).ToDouble();
                var rushYards = curNode.ChildNodes[4].InnerText.Replace(",", string.Empty).ToInt();
                var rushYardsPerGame = curNode.ChildNodes[5].InnerText.Replace(",", string.Empty).ToDouble();
                var passYards = curNode.ChildNodes[6].InnerText.Replace(",", string.Empty).ToInt();
                var passYardsPerGame = curNode.ChildNodes[7].InnerText.Replace(",", string.Empty).ToDouble();
                var totalYards = curNode.ChildNodes[8].InnerText.Replace(",", string.Empty).ToInt();
                var yardsPerGame = curNode.ChildNodes[9].InnerText.Replace(",", string.Empty).ToDouble();

                stats.Add(new TeamStats()
                {
                    LineType = lineType,
                    TeamName = name,
                    Season = season,
                    GamesPlayed = gamesPlayed,
                    TotalYards = totalYards,
                    TotalPoints = totalPoints,
                    PassYardsPerGame = passYardsPerGame,
                    PassYardsTotal = passYards,
                    PointsPerGame = pointsPerGame,
                    RushPerGame = rushYardsPerGame,
                    RushYardsTotal = rushYards,
                    YardsPerGame = yardsPerGame
                });

            }

            await db.AddRangeAsync(stats);
            await db.SaveChangesAsync();
            return Result<List<TeamStats>, SystemError<TeamDataService>>.Ok(stats);
        }

        #endregion

        #region GET TEAM STATS

        public async Task<Result<TeamStats, SystemError<TeamDataService>>> GetTeamStatsAsync(int choice, int season, string teamName)
        {

            var statList = await jsonDataService.GetTeamStatsFromJsonAsync(choice, season, teamName);
            if (statList.IsOk)
            {
                return Result<TeamStats, SystemError<TeamDataService>>.Ok(statList.Value);
            }

            HtmlNodeCollection? nodes = null;
            var link = "";
            var lineType = choice == 0 ? LineType.Offense : LineType.Defense;
            var web = new HtmlWeb();
            HtmlDocument? doc;

            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            switch (choice)
            {
                case 0:
                    teamName = textInfo.ToTitleCase(teamName);
                    link = $"https://www.footballdb.com/statistics/nfl/team-stats/offense-totals/{season}/regular-season?sort=ydsgm";
                    doc = web.Load(link);
                    nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                    break;
                case 1:
                    teamName = textInfo.ToTitleCase(teamName);
                    link = $"https://www.footballdb.com/statistics/nfl/team-stats/defense-totals/{season}/regular-season?sort=ydsgm";
                    doc = web.Load(link);
                    nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                    break;
                default:
                    break;
            }
            
            if (nodes is null) return Result<TeamStats, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
            {
                ErrorMessage = $"No stats found for {teamName}",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });

            foreach (var curNode in nodes)
            {
                if (!curNode.HasChildNodes) continue;
                if (curNode.ChildNodes[0].ChildNodes[1].InnerText != teamName) continue;

                var name = curNode.ChildNodes[0].ChildNodes[1].InnerText;
                var gamesPlayed = curNode.ChildNodes[1].InnerText.ToInt();
                var totalPoints = curNode.ChildNodes[2].InnerText.ToInt();
                var pointsPerGame = curNode.ChildNodes[3].InnerText.Replace(",", string.Empty).ToDouble();
                var rushYards = curNode.ChildNodes[4].InnerText.Replace(",", string.Empty).ToInt();
                var rushYardsPerGame = curNode.ChildNodes[5].InnerText.Replace(",", string.Empty).ToDouble();
                var passYards = curNode.ChildNodes[6].InnerText.Replace(",", string.Empty).ToInt();
                var passYardsPerGame = curNode.ChildNodes[7].InnerText.Replace(",", string.Empty).ToDouble();
                var totalYards = curNode.ChildNodes[8].InnerText.Replace(",", string.Empty).ToInt();
                var yardsPerGame = curNode.ChildNodes[9].InnerText.Replace(",", string.Empty).ToDouble();

                var stats = new TeamStats()
                {
                    TeamName = name,
                    LineType = lineType,
                    Season = season,
                    GamesPlayed = gamesPlayed,
                    TotalYards = totalYards,
                    TotalPoints = totalPoints,
                    PassYardsPerGame = passYardsPerGame,
                    PassYardsTotal = passYards,
                    PointsPerGame = pointsPerGame,
                    RushPerGame = rushYardsPerGame,
                    RushYardsTotal = rushYards,
                    YardsPerGame = yardsPerGame
                };

                return Result<TeamStats, SystemError<TeamDataService>>.Ok(stats);
            } 

            return Result<TeamStats, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
            {
                ErrorMessage = $"No stats found for {teamName}",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });

        }

        #endregion

        #region GET ALL TEAM STANDININGS
        public async Task<Result<List<TeamStanding>, SystemError<TeamDataService>>> GetAllTeamStandings(int season)
        {
            var foundStandings = await jsonDataService.GetStandingsFromJsonAsync(season);
            if (foundStandings is { IsOk: true, Value.Count: > 0 })
                return Result<List<TeamStanding>, SystemError<TeamDataService>>.Ok(foundStandings.Value);
            
            var link = $"https://www.footballdb.com/standings/index.html?lg=NFL&yr={season}";
            var web = new HtmlWeb();
            var doc = web.Load(link);

            var statTableNodes = doc.DocumentNode.SelectNodes(".//table[@class='statistics']");

            if (statTableNodes is null && statTableNodes!.Count != 8)
            {
                return Result<List<TeamStanding>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
                {
                    ErrorMessage = "No standings found for the given season.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }

            var nodeList = statTableNodes.Select(statNode => 
                ParseStandingNode(statNode, season))
                .Where(parsedNode => parsedNode.IsOk)
                .SelectMany(parsedNode => parsedNode.Value).ToList();

            var jsonFound = await jsonDataService.WriteStandingsToJsonAsync(nodeList, season);

            return Result<List<TeamStanding>, SystemError<TeamDataService>>.Ok(nodeList);
        }
        #endregion

        #region PARSE STANDING NODE
        private Result<List<TeamStanding>, SystemError<TeamDataService>> ParseStandingNode(HtmlNode node, int season)
        {
            var childNodes = node.SelectNodes(".//tbody//tr");
            //var pattern = @"^\S+(\s\S+)$";
            var standingList = (from curNode in childNodes
                where curNode.HasChildNodes
                let teamName = curNode.ChildNodes[0].ChildNodes[1].InnerText.ToShortName()
                let wins = curNode.ChildNodes[1].InnerText
                let loses = curNode.ChildNodes[2].InnerText
                let ties = curNode.ChildNodes[3].InnerText
                let pct = curNode.ChildNodes[4].InnerText
                select new TeamStanding
                {
                    Season = season,
                    TeamName = teamName,
                    Abbr = teamName.ToAbbr(),
                    Division = teamName.ToDivision(),
                    Wins = wins,
                    Loses = loses,
                    Ties = ties,
                    Pct = pct
                }).ToList();

            if (standingList.Count == 4)
                return Result<List<TeamStanding>, SystemError<TeamDataService>>.Ok(standingList);

            return Result<List<TeamStanding>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
            {
                ErrorMessage = "could not parse given node.",
                ErrorType = ErrorType.WARNING,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }
        #endregion
    }
}
