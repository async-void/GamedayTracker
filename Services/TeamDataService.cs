using System.Diagnostics;
using System.Globalization;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Services
{
    public class TeamDataService : ITeamData
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

        #region GET DRAFT RESULT FOR TEAM
        public async Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultForTeamAsync(int year, string tName)
        {
            await using var db = new AppDbContextFactory().CreateDbContext();
            var entityList = new List<DraftEntity>();
            var dbList = await db.DraftEntities.Where(x => x.Season == year && x.TeamName.Equals(tName)).ToListAsync();

            if (dbList.Count > 0)
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(dbList);

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
            db.DraftEntities.AddRange(entityList);
            await db.SaveChangesAsync();
            return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(entityList);
        }
        #endregion

        #region GET DRAFT RESULTS
        public async Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultsAsync(int year)
        {
            await using var db = new AppDbContextFactory().CreateDbContext();
            var entityList = new List<DraftEntity>();
            var dbList = await db.DraftEntities.Where(x => x.Season == year).ToListAsync();

            if (dbList.Count > 0) 
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(dbList);

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
            db.DraftEntities.AddRange(entityList);
            await db.SaveChangesAsync();
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
                "New england", 
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
            var statList = db.TeamStats.Where(x => x.Season == season).ToList();
            if (statList.Count > 0)
            {
                return Result<List<TeamStats>, SystemError<TeamDataService>>.Ok(statList);
            }

            var link = $"https://www.footballdb.com/statistics/nfl/team-stats/offense-totals/{season}/regular-season";
            var web = new HtmlWeb();
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

            for (int i = 0; i < nodes.Count; i++)
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
            await using var db = new AppDbContextFactory().CreateDbContext();
            HtmlNodeCollection? nodes = null;
            var link = "";
            var web = new HtmlWeb();
            HtmlDocument? doc = null;
            var statList = new List<TeamStats>();
            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            switch (choice)
            {
                case 0:
                    teamName = textInfo.ToTitleCase(teamName);

                    statList = db.TeamStats.Where(x => x.Season == season && x.TeamName!.Equals(teamName)).ToList();
                    if (statList.Count > 0)
                    {
                        return Result<TeamStats, SystemError<TeamDataService>>.Ok(statList.First());
                    }

                    link = $"https://www.footballdb.com/statistics/nfl/team-stats/offense-totals/{season}/regular-season?sort=ydsgm";
                    doc = web.Load(link);
                    nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                    break;
                case 1:
                    teamName = textInfo.ToTitleCase(teamName);

                    statList = db.TeamStats.Where(x => x.Season == season && x.TeamName!.Equals(teamName)).ToList();
                    if (statList.Count > 0)
                    {
                        return Result<TeamStats, SystemError<TeamDataService>>.Ok(statList.First());
                    }

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

            for (int i = 0; i < nodes.Count; i++)
            {
                var curNode = nodes[i];
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
                await db.TeamStats.AddAsync(stats);
                await db.SaveChangesAsync();
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

    }
}
