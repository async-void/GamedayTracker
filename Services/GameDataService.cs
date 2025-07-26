using ChalkDotNET;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Serilog;
using GamedayTracker.Utility;


namespace GamedayTracker.Services
{
    public class GameDataService(IJsonDataService jsonDataService) : IGameData
    {
       
        #region GET CURRENT WEEK
        public Result<int, SystemError<GameDataService>> GetCurWeek()
        {
            const string link = "https://www.footballdb.com/scores/index.html";
            var web = new HtmlWeb();
            var doc = web.Load(link);
            var weekNode = doc.DocumentNode.SelectSingleNode(".//h2");
            var week = weekNode?.InnerText;
            var weekResult = int.TryParse(week, out var wResult);

            if (weekResult)
            {
                return Result<int, SystemError<GameDataService>>.Ok(wResult);
            }
            return Result<int, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
            {
                ErrorMessage = "Unable to get current week",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }

        #endregion

        #region GET CURRENT SEASON
        public Result<int, SystemError<GameDataService>> GetCurSeason()
        {
            const string link = "https://www.footballdb.com/standings/index.html";
            var web = new HtmlWeb();
            var doc = web.Load(link);
            var seasonNode = doc.DocumentNode.SelectSingleNode(".//button[@id='dropdownMenuYear']");

            if (seasonNode is null)
            {
                return Result<int, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("94807acb-8869-4648-a05d-c258af989e2f")),
                    ErrorCode = Guid.Parse("94807acb-8869-4648-a05d-c258af989e2f"),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            var seasonResult = int.TryParse(seasonNode.InnerText.Replace("\n", string.Empty).Trim(), out int season);
            if (seasonResult)
            {
                return season;
            }
            return 0;
        }
        #endregion

        #region GET CURRENT SCOREBOARD
        public Result<List<Matchup>, SystemError<GameDataService>> GetCurrentScoreboard()
        {
            const string scoreboardLink = "https://www.footballdb.com/scores/index.html";
            var web = new HtmlWeb();
            var doc = web.Load(scoreboardLink);
            var gameNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

            if (gameNodes is null)
                return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
                {
                    ErrorMessage = "Unable to get current scoreboard",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            var matchups = new List<Matchup>();

            foreach (var node in gameNodes)
            {
                var dateNode = node.SelectSingleNode("thead/tr/th");
                var scoreBoardNode = node.SelectNodes("tbody/tr");

                var awayScoreValue = scoreBoardNode[0].ChildNodes.Last().InnerText;
                var homeScoreValue = scoreBoardNode[1].ChildNodes.Last().InnerText;
                var awayNode = scoreBoardNode[0].ChildNodes[1];
                var homeNode = scoreBoardNode[1].ChildNodes[1];

                var matchup = new Matchup
                {
                    Week = GetCurWeek().Value,
                    Season = DateTime.UtcNow.Year,
                    GameDate = dateNode!.InnerText,
                    Opponents = new Opponent
                    {
                        AwayTeam = new Team
                        {
                            Name = awayNode.ChildNodes[0].InnerText.Trim(),
                            Abbreviation = awayNode.ChildNodes[0].InnerText.ToAbbr(),
                            Division = awayNode.ChildNodes[0].InnerText.Trim().ToDivision(),
                            Emoji = NflEmojiService.GetEmoji(awayNode.ChildNodes[0].InnerText.Trim().ToAbbr()),
                            LogoPath = LogoPathService.GetLogoPath(awayNode.ChildNodes[0].InnerText.Trim().ToAbbr()),
                            Record = awayNode.ChildNodes[0].InnerText.Trim(),
                            Score = int.TryParse(awayScoreValue, out var awayScore) ? awayScore : 0
                        },
                        HomeTeam = new Team
                        {
                            Name = homeNode.ChildNodes[0].InnerText.Trim(),
                            Abbreviation = homeNode.ChildNodes[0].InnerText.ToAbbr(),
                            Division = homeNode.ChildNodes[0].InnerText.Trim().ToDivision(),
                            Emoji = NflEmojiService.GetEmoji(homeNode.ChildNodes[0].InnerText.Trim().ToAbbr()),
                            LogoPath = LogoPathService.GetLogoPath(homeNode.ChildNodes[0].InnerText.Trim().ToAbbr()),
                            Record = homeNode.ChildNodes[0].InnerText.Trim(),
                            Score = int.TryParse(homeScoreValue, out var homeScore) ? homeScore : 0
                        }
                    }
                    
                };
                matchups.Add(matchup);
            }
            return Result<List<Matchup>, SystemError<GameDataService>>.Ok(matchups);
        }
        #endregion

        /// <summary>
        /// Get Scoreboard for season and week
        /// </summary>
        /// <param name="season"></param>
        /// <param name="week"></param>
        /// <returns>List Matchup</returns>
        #region GET SCOREBOARD
        public async Task<Result<List<Matchup>, SystemError<GameDataService>>> GetScoreboard(int season, int week)
        {
            var matchups = new List<Matchup>();
            var sw = new Stopwatch();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"matchups_{season}.json");
            
            if (File.Exists(filePath))
            {
                sw.Start();
                var jsonFound = await jsonDataService.GetMatchupsAsync(season.ToString(), week.ToString());
                sw.Stop();
               

                if (jsonFound.IsOk)
                    return Result<List<Matchup>, SystemError<GameDataService>>.Ok(jsonFound.Value);
 
                return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>()
                {
                    ErrorMessage = "Something went wrong while fetching the XML data!",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
                
            }
            sw.Start();
           

            for (var j = 1; j < 23; j++)
            {
                var link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=reg&wk={j}";

                if (j > 18)
                {
                    switch (j)
                    {
                        case 19:
                            link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=post&wk=1";
                            break;
                        case 20:
                            link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=post&wk=2";
                            break;
                        case 21:
                            link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=post&wk=3";
                            break;
                        case 22:
                            link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=post&wk=4";
                            break;
                        default:
                            continue;
                    }
                }

                var web = new HtmlWeb();
                var doc = web.Load(link);

                var scoreboardNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

                if (scoreboardNodes is null) continue;
                   
                for (var i = 0; i <= scoreboardNodes.Count - 1; i++)
                {
                    var node = scoreboardNodes[i].ChildNodes[3];
                    if (!node.HasChildNodes) continue;
                    try
                    {
                        var matchup = ParseMatchup(node, season, j);

                        if (matchup.IsOk)
                            matchups.Add(matchup.Value);
                        else
                        {
                            
                        }
                    }
                    catch (Exception e)
                    {
                        var error = new SystemError<GameDataService>()
                        {
                            ErrorMessage = e.Message,
                            CreatedBy = this,
                            CreatedAt = DateTime.UtcNow,
                            ErrorType = Enums.ErrorType.INFORMATION,
                        };
                        
                    }
                }
               
            }

            //xmlData.WriteAllMatchupsToXmlAsync(matchups, season.ToString()).Wait();
            for (int i = 0; i < matchups.Count; i++)
            {
                matchups[i].Id = i + 1;
            }
            
            try
            {
                await jsonDataService.WriteAllMatchupsToJson(matchups, season);
            }
            catch (Exception e)
            {
               Serilog.Log.Information($"An error occurred while writing matchups to JSON: {e.Message}");
            }
            

            sw.Stop();
            
            return Result<List<Matchup>, SystemError<GameDataService>>.Ok(matchups.Where(m => m.Week.Equals(week)).ToList());

        }
        #endregion

        /// <summary>
        /// Parse Matchup Node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="season"></param>
        /// <param name="week"></param>
        /// <returns>Matchup</returns>
        #region PARSE MATCHUP

        public Result<Matchup, SystemError<GameDataService>> ParseMatchup(HtmlNode node, int season, int week)
        {
            try
            {
                var awayNode = node.ChildNodes[1];
                var awayName = awayNode.ChildNodes[1].InnerText;
                var homeNode = node.ChildNodes[3];
                var homeName = homeNode.ChildNodes[1].InnerText;
                var awayMatch = Regex.Match(awayName, """(?<name>[\w\s]+) (?<rec>\(\d+-\d+\))""");
                var homeMatch = Regex.Match(homeName, """(?<name>[\w\s]+) (?<rec>\(\d+-\d+\))""");

                var awayNameFinal = awayMatch.Groups["name"];
                var awayAbbr = awayNameFinal.Value.ToAbbr();
                var awayRecord = awayMatch.Groups["rec"];

                var homeNameFinal = homeMatch.Groups["name"];
                var homeAbbr = homeNameFinal.Value.ToAbbr();
                var homeRecord = homeMatch.Groups["rec"];

                var awayScore = awayNode.LastChild.InnerText;
                var homeScore = homeNode.LastChild.InnerText;

                var awayTeam = new Team()
                {
                    Name = awayNameFinal.Value,
                    Score = int.Parse(awayScore),
                    Record = awayRecord.Value,
                    Division = awayNameFinal.Value.ToDivision(),
                    Abbreviation = awayAbbr,
                    LogoPath = LogoPathService.GetLogoPath(awayAbbr),
                    Emoji = NflEmojiService.GetEmoji(awayAbbr)
                };

                var homeTeam = new Team()
                {
                    Name = homeNameFinal.Value,
                    Score = int.Parse(homeScore),
                    Record = homeRecord.Value,
                    Division = homeNameFinal.Value.ToDivision(),
                    Abbreviation = homeAbbr,
                    LogoPath = LogoPathService.GetLogoPath(homeAbbr),
                    Emoji = NflEmojiService.GetEmoji(homeAbbr)
                };
                var matchup = new Matchup()
                {
                    Week = week,
                    Season = season,
                    Opponents = new Opponent { AwayTeam = awayTeam, HomeTeam = homeTeam }
                };


                return Result<Matchup, SystemError<GameDataService>>.Ok(matchup);
            }
            catch (Exception)
            {
                var error = new SystemError<GameDataService>
                {
                    ErrorType = ErrorType.INFORMATION,
                    ErrorMessage = "An Error occurred while parsing the matchup data!",
                    CreatedBy = this,
                    CreatedAt = DateTime.UtcNow,
                };
                return Result<Matchup, SystemError<GameDataService>>.Err(error);
            }
        }

        #endregion

        /// <summary>
        /// get the matchup count for a specific week
        /// </summary>
        /// <param name="season"></param>
        /// <param name="week"></param>
        /// <returns>int</returns>
        #region GET MATCHUP COUNT

        public int GetMatchupCount(int season, int week)
        {
            var mainLink = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type=reg&wk={week}";
            var web = new HtmlWeb();
            var doc = web.Load(mainLink);
            var scoreNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

            return scoreNodes?.Count ?? 0;
        }

        #endregion

        /// <summary>
        /// get the team schedule
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="season"></param>
        /// <returns>Task</returns>
        #region GET TEAM SCHEDULE
        public async Task<Result<List<Matchup>, SystemError<GameDataService>>> GetTeamSchedule(string teamName)
        {
            var schedule = await jsonDataService.GetSeasonScheduleFromJsonAsync(DateTime.UtcNow.Year, teamName);

            if (schedule.IsOk)
            {
                return Result<List<Matchup>, SystemError<GameDataService>>.Ok(schedule.Value);
            }

            var scheduleList = new List<Matchup>();
            var season = DateTime.UtcNow.Year;
            var teamLinkName = teamName.ToTeamLinkName();
            var scheduleLink = $"https://www.footballdb.com/teams/nfl/{teamLinkName}/results";
            var web = new HtmlWeb();
            var doc = web.Load(scheduleLink);
            var scheduleNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

            if (scheduleNodes is null)
                return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
                {
                    ErrorMessage = $"no schedule found for ``{teamName}``",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });

            for (var i = 0; i < scheduleNodes.Count; i++)
            {
                var curNode = scheduleNodes[i];
                var curDate = "";
                if (!curNode.HasChildNodes) continue;
         
                foreach (var childNode in curNode.ChildNodes)
                {
                    if (childNode.Name.Equals("thead"))
                    {
                        var dateNode = childNode.SelectSingleNode(".//tr/th");
                        if (dateNode is not null)
                        {
                            curDate = dateNode.InnerText.Trim();
                            curDate = curDate.Replace(" ", "/").Replace(",", string.Empty);
                            string format = "dddd/MMMM/d/yyyy";
                            CultureInfo provider = CultureInfo.InvariantCulture;

                            if (DateTime.TryParseExact(curDate, format, provider, DateTimeStyles.None, out DateTime result))
                            {
                                curDate = result.ToLongDateString();
                            }  
                        }
                    }
                    else if (childNode.Name.Equals("tbody"))
                    {
                        var bodyNode = childNode;
                        if (bodyNode.HasChildNodes)
                        {
                            var vsAwayName = bodyNode.ChildNodes[1].InnerText.Replace("\n", string.Empty).Replace("(0-0)", string.Empty).Replace("--", string.Empty).Trim(); ;
                            var vsHomeName = bodyNode.ChildNodes[3].InnerText.Replace("\n", string.Empty).Replace("(0-0)", string.Empty).Replace("--", string.Empty).Trim();
                            var vsAwayAbbr = vsAwayName.ToAbbr();
                            var vsHomeAbbr = vsHomeName.ToAbbr();
                            var awayEmoji = NflEmojiService.GetEmoji(vsAwayAbbr);
                            var homeEmoji = NflEmojiService.GetEmoji(vsHomeAbbr);
                            var awayDivision = vsAwayName.ToDivision();
                            var homeDivision = vsHomeName.ToDivision();

                            var awayTeam = new Team {Division = awayDivision, Record = "(0-0)", Abbreviation = vsAwayAbbr, Name = vsAwayName, Emoji = awayEmoji, LogoPath = "" };
                            var homeTeam = new Team {Division = homeDivision, Record = "(0-0)", Abbreviation = vsHomeAbbr, Name = vsHomeName, Emoji = homeEmoji, LogoPath = "" };
                            var matchup = new Matchup
                            {
                                Season = season,
                                GameDate = curDate,
                                Week = i + 1,
                                Opponents = new Opponent
                                {
                                    AwayTeam = awayTeam,
                                    HomeTeam = homeTeam
                                }
                            };
                            scheduleList.Add(matchup);
                        }
                    } 
                }
            }
            await jsonDataService.WriteSeasonScheduleToJson(scheduleList, teamName);
            return Result<List<Matchup>, SystemError<GameDataService>>.Ok(scheduleList);
        }

        #endregion

    }
}
