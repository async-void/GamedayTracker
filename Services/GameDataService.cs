using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL;
using GamedayTracker.Utility;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace GamedayTracker.Services
{
    public class GameDataService(IJsonDataService jsonDataService, ILogger<GameDataService> logger) : IGameData
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ESPN_API = "http://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard";

        // Add this as a private static readonly field at the top of the class (after the httpClient field)
        private static readonly JsonSerializerOptions CachedJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };


        #region GET CURRENT WEEK
        public Result<int, SystemError<GameDataService>> GetCurWeek()
        {
            const string link = "https://www.footballdb.com/scores/index.html";
            var web = new HtmlWeb();
            var doc = web.Load(link);

            var buttonNode = doc.DocumentNode.SelectNodes("//button[@id='dropdownMenuLeague']");

            var weekTextNode = buttonNode.Select(x => x.SelectSingleNode(".//span[contains(text(), 'Week')]")).ToList();

            var week = weekTextNode[1]?.InnerText ?? "1";
            var parsedWeek = Regex.Replace(week, @"\D", string.Empty);
            var weekResult = int.TryParse(parsedWeek, out var wResult);

            if (weekResult)
            {
                return Result<int, SystemError<GameDataService>>.Ok(wResult);
            }
            return Result<int, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
            {
                ErrorMessage = "Unable to get current week - html node not found",
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
        public async Task<Result<List<Matchup>, SystemError<GameDataService>>> GetCurrentScoreboard()
        {
            var week = GetCurWeek();
            var scoreboardLink = "https://www.footballdb.com/scores/index.html";
            //var scoreboardLink = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr=2025&type=reg&wk={week.Value}";
            var web = new HtmlWeb();
            var doc = web.Load(scoreboardLink);
            var gameNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

            if (gameNodes is null || !gameNodes.Any())
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
                var dateNode = node.SelectSingleNode("thead/tr");
                var gameTimeNode = node.SelectSingleNode("thead/tr");
                var scoreBoardNode = node.SelectNodes("tbody/tr");

                if (scoreBoardNode is null)
                    return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
                    {
                        ErrorCode = Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b"),
                        ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b")),
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });

                //scoreBoardNode is not null, proceed.
                var awayScoreValue = scoreBoardNode[0].ChildNodes.Last().InnerText;
                var homeScoreValue = scoreBoardNode[1].ChildNodes.Last().InnerText;
                var awayNode = scoreBoardNode[0].ChildNodes[1];
                var homeNode = scoreBoardNode[1].ChildNodes[1];

                var gameDate = dateNode?.ChildNodes[0].InnerText.Trim();
                //var gameTime = gameTimeNode?.ChildNodes[1].InnerText.Trim();

                var matchup = new Matchup
                {
                    Week = GetCurWeek().Value,
                    Season = DateTime.UtcNow.Year,
                    GameDate = gameDate.ToString(),
                    GameTime = "",
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

                //return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>()
                //{
                //    ErrorMessage = "Something went wrong while fetching the Scoreboard data!",
                //    ErrorType = ErrorType.INFORMATION,
                //    CreatedAt = DateTime.UtcNow,
                //    CreatedBy = this
                //});

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

                if (scoreboardNodes is null)
                {
                    return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>()
                    {
                        ErrorMessage = $"No data found for Season: {season} Week: {week}",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = this
                    });
                }

                for (var i = 0; i <= scoreboardNodes.Count - 1; i++)
                {
                    var node = scoreboardNodes[i].ChildNodes[3];
                    if (!node.HasChildNodes) continue;
                    try
                    {
                        var matchup = ParseMatchup(node, season, j);

                        if (matchup.IsOk)
                            matchups.Add(matchup.Value);

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

            return Result<List<Matchup>, SystemError<GameDataService>>.Ok([.. matchups.Where(m => m.Week.Equals(week) && m.Season.Equals(season))]);

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
                    Score = int.TryParse(awayScore, out var finalAwayScore) ? finalAwayScore : 0,
                    Record = awayRecord.Value,
                    Division = awayNameFinal.Value.ToDivision(),
                    Abbreviation = awayAbbr,
                    LogoPath = LogoPathService.GetLogoPath(awayAbbr),
                    Emoji = NflEmojiService.GetEmoji(awayAbbr),
                };

                var homeTeam = new Team()
                {
                    Name = homeNameFinal.Value,
                    Score = int.TryParse(homeScore, out var finalHomeScore) ? finalHomeScore : 0,
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

                            var awayTeam = new Team { Division = awayDivision, Record = "(0-0)", Abbreviation = vsAwayAbbr, Name = vsAwayName, Emoji = awayEmoji, LogoPath = "" };
                            var homeTeam = new Team { Division = homeDivision, Record = "(0-0)", Abbreviation = vsHomeAbbr, Name = vsHomeName, Emoji = homeEmoji, LogoPath = "" };
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

        #region GET NFL SCORES
        public async Task<NFLScoreboard> GetNFLScoresAsync(int? season = null, int? week = null, int? seasonType = null)
        {
            try
            {
                string url = ESPN_API;
                var newWeek = ConvertWeekBySeasonType(week ?? 0, seasonType ?? 2);
                if (season.HasValue && week.HasValue)
                {
                    // ESPN API format for specific weeks - UPDATE YEAR AS NEEDED
                    url = $"http://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard?dates={season}&seasontype={seasonType}&week={newWeek}";
                }
                if (season.HasValue && !week.HasValue)
                {
                    // If only season is provided, get the full season scoreboard
                    url = $"http://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard?dates={season}&{seasonType}";
                }

                var response = await httpClient.GetStringAsync(url);
                var scores = JsonSerializer.Deserialize<NFLScoreboard>(response);

                httpClient.Dispose();
                return scores;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching scores: {ex.Message}");
                httpClient.Dispose();
                return null;
            }
        }
        #endregion

        #region IS GAME IN PROGRESS
        // Check if a game is in progress
        public bool IsGameInProgress(Competition competition)
        {
            return competition.Status.Type.State == "in";
        }
        #endregion

        #region IS GAME COMPLETED
        // Check if a game is completed
        public bool IsGameCompleted(Competition competition)
        {
            return competition.Status.Type.Completed;
        }
        #endregion

        #region IS GAME SCHEDULED
        // Check if a game is scheduled (not started yet)
        public bool IsGameScheduled(Competition competition)
        {
            return competition.Status.Type.State == "pre";
        }
        #endregion

        #region GET GAME STATUS
        // Get game status description
        public string GetGameStatus(Competition competition)
        {
            var status = competition.Status.Type;

            if (status.Completed)
            {
                return "Final";
            }
            else if (status.State == "in")
            {
                // Game is live
                var quarter = GetQuarterName(competition.Status.Period);
                var clock = competition.Status.DisplayClock;
                return $"Live - {quarter} {clock}";
            }
            else if (status.State == "pre")
            {
                // Game hasn't started
                return status.ShortDetail; // e.g., "9/22 - 1:00 PM EDT"
            }

            return status.Detail;
        }
        #endregion

        #region GET QUARTER NAME
        // Get quarter/period name
        public string GetQuarterName(int period)
        {
            return period switch
            {
                1 => "Q1",
                2 => "Q2",
                3 => "Q3",
                4 => "Q4",
                5 => "OT",
                _ => $"OT{period - 4}"
            };
        }
        #endregion

        #region GET GAME INFO
        // Get detailed game info
        public async Task<string> GetGameInfo(Event game)
        {
            var competition = game.Competitions[0];
            var homeTeam = competition.Competitors.FirstOrDefault(c => c.HomeAway == "home");
            var awayTeam = competition.Competitors.FirstOrDefault(c => c.HomeAway == "away");

            if (homeTeam == null || awayTeam == null)
                return "Unable to load game info";

            var info = $"**{awayTeam.Team.DisplayName}** @ **{homeTeam.Team.DisplayName}**\n";

            if (IsGameCompleted(competition))
            {
                // Final score
                info += $"**Final:** {awayTeam.Team.Abbreviation} {awayTeam.Score} - {homeTeam.Score} {homeTeam.Team.Abbreviation}";

                // Show winner
                var winner = awayTeam.Winner ? awayTeam.Team.Abbreviation : homeTeam.Team.Abbreviation;
                info += $" (Winner: {winner})";
            }
            else if (IsGameInProgress(competition))
            {
                // Live score with quarter and time
                var quarter = GetQuarterName(competition.Status.Period);
                var clock = competition.Status.DisplayClock;

                info += $"**{quarter} - {clock}**\n";
                info += $"{awayTeam.Team.Abbreviation} {awayTeam.Score} - {homeTeam.Score} {homeTeam.Team.Abbreviation}";

                // Show line scores (quarter by quarter)
                info += "\n\n**By Quarter:**\n";
                info += GetLineScores(awayTeam, homeTeam);
            }
            else
            {
                // Scheduled game
                info += $"**Scheduled:** {competition.Status.Type.ShortDetail}";

                // Show records
                var awayRecord = await GetTeamRecordAsync(awayTeam.Team.Abbreviation);
                var homeRecord = await GetTeamRecordAsync(homeTeam.Team.Abbreviation);

                if (!string.IsNullOrEmpty(awayRecord.Item1) && !string.IsNullOrEmpty(homeRecord.Item1))
                {
                    info += $"\n{awayTeam.Team.Abbreviation}: {awayRecord.Item1} | {homeTeam.Team.Abbreviation}: {homeRecord.Item1}";
                }
            }

            return info;
        }
        #endregion

        #region GET LINE SCORES
        // Get quarter-by-quarter scores
        public string GetLineScores(Competitor away, Competitor home)
        {
            if (away.LineScores == null || home.LineScores == null)
                return "";

            var result = $"{away.Team.Abbreviation}: ";
            result += string.Join(" | ", away.LineScores.Select(ls => $"Q{ls.Period}: {ls.DisplayValue}"));
            result += $"\n{home.Team.Abbreviation}: ";
            result += string.Join(" | ", home.LineScores.Select(ls => $"Q{ls.Period}: {ls.DisplayValue}"));

            return result;
        }
        #endregion

        #region GET LIVE GAMES
        // Get all live games
        public List<Event> GetLiveGames(NFLScoreboard scoreboard)
        {
            return [.. scoreboard.Events.Where(e => IsGameInProgress(e.Competitions[0]))];
        }
        #endregion

        #region GET COMPLETED GAMES
        // Get all completed games
        public List<Event> GetCompletedGames(NFLScoreboard scoreboard)
        {
            return [.. scoreboard.Events.Where(e => IsGameCompleted(e.Competitions[0]))];
        }
        #endregion

        #region GET SCHEDULED GAMES
        // Get all scheduled games
        public List<Event> GetScheduledGames(NFLScoreboard scoreboard)
        {
            return [.. scoreboard.Events.Where(e => IsGameScheduled(e.Competitions[0]))];

        }
        #endregion

        #region GET GAME LEADERS
        // Get leading stats for a game
        public string GetGameLeaders(Competition competition)
        {
            if (competition.Leaders == null || competition.Leaders.Count == 0)
                return "No stats available";

            var result = "**Game Leaders:**\n";

            foreach (var leaderCategory in competition.Leaders)
            {
                if (leaderCategory.Leaders != null && leaderCategory.Leaders.Count > 0)
                {
                    var leader = leaderCategory.Leaders[0];
                    result += $"\n**{leaderCategory.ShortDisplayName}:** ";
                    result += $"{leader.Athlete.ShortName} - {leader.DisplayValue}";
                }
            }

            return result;
        }
        #endregion

        #region GET TEAM GAMES
        // Get all games for a specific team
        public List<Event> GetTeamGames(NFLScoreboard scoreboard, string teamAbbreviation)
        {
            return [.. scoreboard.Events
                .Where(e =>
                {
                    var competition = e.Competitions[0];
                    return competition.Competitors.Any(c =>
                        c.Team.Abbreviation.Equals(teamAbbreviation, StringComparison.OrdinalIgnoreCase));
                })];
        }
        #endregion

        #region GET TEAM RECORD
        public async Task<(string, NFLTeam)> GetTeamRecordAsync(string teamAbbr)
        {
            var BaseUrl = $"https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/";
            try
            {
                var url = $"{BaseUrl}{teamAbbr}";
                var response = await httpClient.GetStringAsync(url);

                var teamData = JsonSerializer.Deserialize<NFLRecordResponse>(response);

                // Get the total record (first item with type "total")
                var totalRecord = teamData.Team.Record.Items[0].Summary ?? "Record not found";
                var homeRecord = teamData.Team.Record.Items[1].Summary ?? "Record not found";
                var awayRecord = teamData.Team.Record.Items[2].Summary ?? "Record not found";

                return (totalRecord, teamData.Team);
            }
            catch (Exception ex)
            {
                return ("error not found", null);
            }
        }
        #endregion

        #region GET SEASON TYPE
        // Get season type from scoreboard
        public int GetSeasonType(NFLScoreboard scoreboard)
        {
            return scoreboard.Season?.Type ?? 2; // Default to regular season (2)
        }
        #endregion

        #region GET SEASON INFO
        // Get full season info
        public string GetSeasonInfo(NFLScoreboard scoreboard)
        {
            var seasonType = GetSeasonType(scoreboard);
            var seasonTypeName = GetSeasonTypeName(seasonType);
            var year = scoreboard.Season?.Year ?? DateTime.Now.Year;
            var week = scoreboard.Week?.Number ?? 0;

            return $"{year} {seasonTypeName} - Week {week}";
        }
        #endregion

        #region GET SEASON TYPE NAME
        // Get season type name
        public string GetSeasonTypeName(int seasonType)
        {
            return seasonType switch
            {
                1 => "Preseason",
                2 => "Regular Season",
                3 => "Postseason",
                4 => "Off Season",
                _ => "Unknown"
            };
        }

        #endregion

        #region GET WEEK DISPLAY NAME
        // Get week display name (handles regular season and playoffs)
        public string GetWeekDisplayName(NFLScoreboard scoreboard)
        {
            var seasonType = GetSeasonType(scoreboard);
            var week = scoreboard.Week?.Number ?? 0;

            if (seasonType == 3) // Postseason
            {
                return GetPlayoffWeekName(scoreboard);
            }
            else if (seasonType == 1) // Preseason
            {
                return $"Preseason Week {week}";
            }
            else // Regular season
            {
                return $"Week {week}";
            }
        }
        #endregion

        #region GET PLAYOFF WEEK NAME
        // Get playoff week name (for postseason)
        public string GetPlayoffWeekName(NFLScoreboard scoreboard)
        {
            var seasonType = GetSeasonType(scoreboard);

            // Only return playoff names if it's postseason
            if (seasonType != 3)
                return null;

            var weekNumber = scoreboard.Week?.Number ?? 0;

            return weekNumber switch
            {
                1 => "Wild Card",
                2 => "Divisional",
                3 => "Conference Championship",
                4 => "Pro Bowl",
                5 => "Super Bowl",
                _ => $"Postseason Week {weekNumber}"
            };
        }
        #endregion

        #region GET FULL WEEK DISPLAY
        // Get full season and week display
        public string GetFullSeasonWeekDisplay(NFLScoreboard scoreboard)
        {
            var year = scoreboard.Season?.Year ?? DateTime.Now.Year;
            var seasonType = GetSeasonType(scoreboard);
            var weekDisplay = GetWeekDisplayName(scoreboard);

            if (seasonType == 3) // Postseason - show just the playoff round name
            {
                return $"{year} {weekDisplay}";
            }
            else
            {
                return $"{year} {GetSeasonTypeName(seasonType)} - {weekDisplay}";
            }
        }
        #endregion

        #region CONVERT WEEK BASED ON SEASON TYPE
        public int ConvertWeekBySeasonType(int week, int seasonType)
        {
            if (seasonType == 3 && week >= 19 && week <= 22)
            {
                return week - 18; // 19->1, 20->2, 21->3, 22->4
            }
            return week;
        }
        #endregion

        #region GET NFL STANDINGS
        public async Task<NflStandings> GetNFLStandingsAsync()
        {
            string ESPN_API_URL = "https://site.api.espn.com/apis/v2/sports/football/nfl/standings";
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(ESPN_API_URL);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var standings = JsonSerializer.Deserialize<NflStandings>(json, CachedJsonOptions);


                logger.LogInformation("Successfully fetched NFL standings.");
                return standings;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error fetching NFL standings: {ex.Message}");
                return null;
            }

        }
        #endregion

    }
}
