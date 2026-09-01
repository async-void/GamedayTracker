using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL;
using GamedayTracker.Models.NFL.InjuryReport;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GamedayTracker.Services
{
    public class TeamDataService(IJsonDataService jsonDataService, HttpClient client, IMemoryCache cache) : ITeamData
    {
        private const string BaseUrl = "https://sports.core.api.espn.com/v2/sports/football/leagues/nfl";

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

        #region GET ID FROM TEAMNAME
        public Result<string, SystemError<TeamDataService>> GetIdFromTeamName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Result<string, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
                {
                    ErrorMessage = "teamname was null or empty.",
                    ErrorType = ErrorType.WARNING,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });

            // Normalize input: trim, lowercase, remove extra spaces
            var normalized = input.Trim().ToLower().Replace("  ", " ");

            return normalized switch
            {
                // Atlanta Falcons - ID: 1
                "falcons" or "atlanta" or "atl" or "atlanta falcons" => "1",

                // Buffalo Bills - ID: 2
                "bills" or "buffalo" or "buf" or "buffalo bills" => "2",

                // Chicago Bears - ID: 3
                "bears" or "chicago" or "chi" or "chicago bears" or "da bears" => "3",

                // Cincinnati Bengals - ID: 4
                "bengals" or "cincinnati" or "cin" or "cincinnati bengals" => "4",

                // Cleveland Browns - ID: 5
                "browns" or "cleveland" or "cle" or "cleveland browns" => "5",

                // Dallas Cowboys - ID: 6
                "cowboys" or "dallas" or "dal" or "dallas cowboys" => "6",

                // Denver Broncos - ID: 7
                "broncos" or "denver" or "den" or "denver broncos" => "7",

                // Detroit Lions - ID: 8
                "lions" or "detroit" or "det" or "detroit lions" => "8",

                // Green Bay Packers - ID: 9
                "packers" or "green bay" or "gb" or "green bay packers" => "9",

                // Tennessee Titans - ID: 10
                "titans" or "tennessee" or "ten" or "tennessee titans" => "10",

                // Indianapolis Colts - ID: 11
                "colts" or "indianapolis" or "ind" or "indianapolis colts" => "11",

                // Kansas City Chiefs - ID: 12
                "chiefs" or "kansas city" or "kc" or "kansas city chiefs" => "12",

                // Las Vegas Raiders - ID: 13
                "raiders" or "las vegas" or "lv" or "oakland" or "las vegas raiders" or "oakland raiders" => "13",

                // Los Angeles Rams - ID: 14
                "rams" or "la rams" or "lar" or "los angeles rams" => "14",

                // Miami Dolphins - ID: 15
                "dolphins" or "miami" or "mia" or "miami dolphins" or "fins" => "15",

                // Minnesota Vikings - ID: 16
                "vikings" or "minnesota" or "min" or "minnesota vikings" => "16",

                // New England Patriots - ID: 17
                "patriots" or "new england" or "ne" or "new england patriots" => "17",

                // New Orleans Saints - ID: 18
                "saints" or "new orleans" or "no" or "new orleans saints" => "18",

                // New York Giants - ID: 19
                "giants" or "ny giants" or "nyg" or "new york giants" => "19",

                // New York Jets - ID: 20
                "jets" or "ny jets" or "nyj" or "new york jets" => "20",

                // Philadelphia Eagles - ID: 21
                "eagles" or "philadelphia" or "phi" or "philadelphia eagles" => "21",

                // Arizona Cardinals - ID: 22
                "cardinals" or "arizona" or "ari" or "arizona cardinals" => "22",

                // Pittsburgh Steelers - ID: 23
                "steelers" or "pittsburgh" or "pit" or "pittsburgh steelers" => "23",

                // Los Angeles Chargers - ID: 24
                "chargers" or "la chargers" or "lac" or "san diego" or "los angeles chargers" or "san diego chargers" => "24",

                // San Francisco 49ers - ID: 25
                "49ers" or "niners" or "san francisco" or "sf" or "san fran" or "san francisco 49ers" => "25",

                // Seattle Seahawks - ID: 26
                "seahawks" or "seattle" or "sea" or "seattle seahawks" => "26",

                // Tampa Bay Buccaneers - ID: 27
                "buccaneers" or "bucs" or "tampa bay" or "tb" or "tampa" or "tampa bay buccaneers" => "27",

                // Washington Commanders - ID: 28
                "commanders" or "washington" or "was" or "washington commanders" => "28",

                // Carolina Panthers - ID: 29
                "panthers" or "carolina" or "car" or "carolina panthers" => "29",

                // Jacksonville Jaguars - ID: 30
                "jaguars" or "jags" or "jacksonville" or "jax" or "jacksonville jaguars" => "30",

                // Baltimore Ravens - ID: 33
                "ravens" or "baltimore" or "bal" or "baltimore ravens" => "33",

                // Houston Texans - ID: 34
                "texans" or "houston" or "hou" or "houston texans" => "34",

                _ => ""
            };
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

        #region GET TEAM INJURIES
        public async Task<Result<List<EspnInjury>, SystemError<TeamDataService>>> GetTeamInjuriesAsync(string userInput)
        {
            try
            {
                // 1. Normalize user input → canonical team name
                if (!userInput.TryResolveTeamToShortName(out var canonical))
                {
                    return Result<List<EspnInjury>, SystemError<TeamDataService>>.Err(
                        new SystemError<TeamDataService>
                        {
                            Id = 100,
                            ErrorCode = Guid.NewGuid(),
                            ErrorMessage = $"Unknown team: '{userInput}'.",
                            CreatedBy = this,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.INFORMATION
                        });
                }

                // 2. Canonical → ESPN team ID
                if (!canonical.TryToEspnTeamId(out var teamId))
                {
                    return Result<List<EspnInjury>, SystemError<TeamDataService>>.Err(
                        new SystemError<TeamDataService>
                        {
                            Id = 101,
                            ErrorCode = Guid.NewGuid(),
                            ErrorMessage = $"No ESPN team ID found for '{canonical}'.",
                            CreatedBy = this,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.INFORMATION
                        });
                }

                // 3. Build ESPN endpoint
                var endpoint = $"https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/{teamId}/injuries";

                // 4. Fetch JSON
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<List<EspnInjury>, SystemError<TeamDataService>>.Err(
                        new SystemError<TeamDataService>
                        {
                            Id = 102,
                            ErrorCode = Guid.NewGuid(),
                            ErrorMessage = $"Failed to fetch injury report for '{canonical}'. Status: {response.StatusCode}",
                            CreatedBy = this,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.WARNING
                        });
                }

                var content = await response.Content.ReadAsStringAsync();

                // 5. Deserialize
                var data = JsonSerializer.Deserialize<EspnInjuryResponse>(content);

                data ??= new EspnInjuryResponse();
                data.Injuries ??= [];

                return Result<List<EspnInjury>, SystemError<TeamDataService>>.Ok(data.Injuries);
            }
            catch (Exception ex)
            {
                return Result<List<EspnInjury>, SystemError<TeamDataService>>.Err(
                    new SystemError<TeamDataService>
                    {
                        Id = 103,
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"Exception while fetching injuries for '{userInput}': {ex.Message}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.FATAL
                    });
            }
        }

        #endregion

        #region GET TEAM STATS

        public async Task<Result<NflTeamStatisticsResponse, SystemError<TeamDataService>>> GetTeamStatsAsync(NFLSeasonType seasonType, int season, string teamName)
        {
            var seasonTypeStr = seasonType switch
            {
                NFLSeasonType.Preseason => "1",
                NFLSeasonType.RegularSeason => "2",
                NFLSeasonType.Playoffs => "3",
                _ => "2"
            };

            var teamId = GetIdFromTeamName(teamName);
            var url = $"{BaseUrl}/seasons/{season}/types/{seasonTypeStr}/teams/{teamId.Value}/statistics";

            try
            {
                var cacheKey = $"nfl_stats_{teamName}_{season}_{(int)seasonType}";

                if (cache.TryGetValue(cacheKey, out NflTeamStatisticsResponse? cachedStats))
                {
                    return Result<NflTeamStatisticsResponse, SystemError<TeamDataService>>.Ok(cachedStats);
                }
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"ESPN API returned {response.StatusCode}. URL: {url}. Response: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var stats = JsonSerializer.Deserialize<NflTeamStatisticsResponse>(json, options);

                if (stats == null)
                {
                    throw new InvalidOperationException("Failed to deserialize response from ESPN API");
                }

                return stats;
            }
            catch (HttpRequestException ex)
            {
                return Result<NflTeamStatisticsResponse, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
                {
                    ErrorMessage = $"Failed to retrieve NFL team statistics for team {teamName}: {ex.Message}",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            catch (TaskCanceledException ex)
            {
                return Result<NflTeamStatisticsResponse, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
                {
                    ErrorMessage = $"Request timed out while retrieving NFL team statistics: {ex.Message}",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            catch (Exception ex)
            {
                return Result<NflTeamStatisticsResponse, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>
                {
                    ErrorMessage = ex.Message,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }   
             

        }

        #endregion

        #region GET ALL TEAM STANDININGS
        public async Task<Result<List<TeamStanding>, SystemError<TeamDataService>>> GetAllTeamStandings(int season)
        {
            //var foundStandings = await jsonDataService.GetStandingsFromJsonAsync(season);
            //if (foundStandings is { IsOk: true, Value.Count: > 0 })
            //    return Result<List<TeamStanding>, SystemError<TeamDataService>>.Ok(foundStandings.Value);
            
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

        #region GET TEAM RECORD
        public async Task<(string, NFLTeam)> GetTeamRecordAsync(string teamAbbr)
        {
            var BaseUrl = $"https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/";
            try
            {
                var url = $"{BaseUrl}{teamAbbr}";
                var response = await client.GetStringAsync(url);

                var teamData = JsonSerializer.Deserialize<NFLRecordResponse>(response);

                var totalRecord = teamData.Team.Record.Items[0].Summary ?? "Record not found";
                var homeRecord = teamData.Team.Record.Items[1].Summary ?? "Record not found";
                var awayRecord = teamData.Team.Record.Items[2].Summary ?? "Record not found";

                return (totalRecord, teamData.Team);
            }
            catch(Exception ex)
            {
                return ("Record not found", null);
            }
        }
        #endregion

    }
}
