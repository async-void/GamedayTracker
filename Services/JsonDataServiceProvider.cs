using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.Services
{
    public class JsonDataServiceProvider : IJsonDataService
    {
        #region GET MATCHUPS
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetMatchupsAsync(string season, string week)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"matchups_{season}.json");

            if (!File.Exists(path))
            {
                var notFound = new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Matchup data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                };
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(notFound);
            }
            var json = await File.ReadAllTextAsync(path);
            var matchups = JsonSerializer.Deserialize<List<Matchup>>(json)!
                .Where(m => m.Season.ToString() == season && m.Week.ToString() == week).ToList();

            if (matchups is not null) return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(matchups);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch matchup data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            };
            return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(error);
        }
        #endregion

        #region WRITE MATCHUP TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteMatchupToJson(Matchup matchup)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "matchups.json");

            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(matchup, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var json = JsonSerializer.Deserialize<List<Matchup>>(file);
                json!.Add(matchup);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(json, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }
        #endregion

        #region WRITE ALL MATCHUPS TO JSON
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> WriteAllMatchupsToJson(List<Matchup> matchups, int season)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"matchups_{season}.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true};
                var json = JsonSerializer.Serialize(matchups, options);
                await File.WriteAllTextAsync(path, json);
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(matchups);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingMatchups = JsonSerializer.Deserialize<List<Matchup>>(file);
                existingMatchups!.AddRange(matchups);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingMatchups, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(existingMatchups);
            }
        }
        #endregion

        #region GET STANDINGS FROM JSON
        public async Task<Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>> GetStandingsFromJsonAsync(int season)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"standings_{season}.json");
            if (!File.Exists(path))
            {
                var notFound = new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Standings data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                };
                return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Err(notFound);
            }
            var json = await File.ReadAllTextAsync(path);
            var standings = JsonSerializer.Deserialize<List<TeamStanding>>(json)!;
            if (standings is not null) return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Ok(standings);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch standings data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            };
            return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Err(error);
        }
        #endregion

        #region WRITE STANDINGS TO JSON
        public async Task<Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>> WriteStandingsToJsonAsync(List<TeamStanding> standings, int season)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"standings_{season}.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(standings, options);
                await File.WriteAllTextAsync(path, json);
                return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Ok(standings);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingStandings = JsonSerializer.Deserialize<List<TeamStanding>>(file) ?? new List<TeamStanding>();
                existingStandings.AddRange(standings);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingStandings, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Ok(existingStandings);
            }
        }
        #endregion

        #region WRITE PLAYER TO JSON

        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WritePlayerToJsonAsync(PoolPlayer player)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "players.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(player, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var players = JsonSerializer.Deserialize<List<PoolPlayer>>(file) ?? new List<PoolPlayer>();
                players.Add(player);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(players, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }


        #endregion

        #region GENERATE PLAYER ID
        public async Task<Result<int, SystemError<JsonDataServiceProvider>>> GeneratePlayerIdAsync()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "players.json");
            if (!File.Exists(path))
            {
                return Result<int, SystemError<JsonDataServiceProvider>>.Ok(1);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var players = JsonSerializer.Deserialize<List<PoolPlayer>>(file) ?? new List<PoolPlayer>();
                var maxId = players.Max(p => p.Id);
                return Result<int, SystemError<JsonDataServiceProvider>>.Ok(maxId + 1);
            }
        }
        #endregion

        #region GENERATE PLAYER IDENTIFIER

        public Result<ulong, SystemError<JsonDataServiceProvider>> GeneratePlayerIdentifier()
        {
            var random = new Random();
            var buffer = new byte[8];
            random.NextBytes(buffer);
            var randomUlong = BitConverter.ToUInt64(buffer, 0);

            return Result<ulong, SystemError<JsonDataServiceProvider>>.Ok(randomUlong);
        }

        #endregion

        #region GET TEAM STATS
        public async Task<Result<TeamStats, SystemError<JsonDataServiceProvider>>> GetTeamStatsFromJsonAsync(int choice, int season, string teamName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"team_stats_{season}.json");
            if (!File.Exists(path)) 
            {
                return Result<TeamStats, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Team stats data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var lineType = choice == 0 ? LineType.Offense : LineType.Defense;
            var stats = JsonSerializer.Deserialize<List<TeamStats>>(json)!
                .Where(x => x.Season == season && x.LineType.Equals(lineType) && x.TeamName == teamName).FirstOrDefault();

            if (stats is not null) return Result<TeamStats, SystemError<JsonDataServiceProvider>>.Ok(stats);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch team stats data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            };
            return Result<TeamStats, SystemError<JsonDataServiceProvider>>.Err(error);
        }
        #endregion

        #region GET ALL TEAM STATS
        public async Task<Result<List<TeamStats>, SystemError<JsonDataServiceProvider>>> GetAllTeamStatsFromJsonAsync(int choice, int season, string teamName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"team_stats_{season}.json");
            if (!File.Exists(path))
            {
                return Result<List<TeamStats>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Team stats data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var lineType = choice == 0 ? LineType.Offense : LineType.Defense;
            var stats = JsonSerializer.Deserialize<List<TeamStats>>(json)!;
               

            if (stats is not null) return Result<List<TeamStats>, SystemError<JsonDataServiceProvider>>.Ok(stats);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch team stats data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            };
            return Result<List<TeamStats>, SystemError<JsonDataServiceProvider>>.Err(error);
        }
        #endregion

        #region WRITE ALL TEAM STATS TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteAllTeamStatsToJsonAsync(List<TeamStats> source)
        {
            var season = source.FirstOrDefault()?.Season ?? 0;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"team_stats_{season}.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(source, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            var file = await File.ReadAllTextAsync(path);
            var stats = JsonSerializer.Deserialize<List<TeamStats>>(file) ?? new List<TeamStats>();
           
            if (stats is not null && stats.Count > 0)
            {
                stats.AddRange(source);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(stats, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to write team stats data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region WRITE TEAM STATS TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteTeamStatsToJsonAsync(TeamStats stats)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"team_stats_{stats.Season}.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(stats, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingStats = JsonSerializer.Deserialize<List<TeamStats>>(file) ?? new List<TeamStats>();
                existingStats.Add(stats);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingStats, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }
        #endregion
    }
}
