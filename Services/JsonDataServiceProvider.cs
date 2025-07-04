﻿using ChalkDotNET;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class JsonDataServiceProvider(ILogger logger) : IJsonDataService
    {
        #region GET MATCHUPS
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetMatchupsAsync(string season, string week)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"matchups_{season}.json");
            var timeStamp = DateTimeOffset.UtcNow;
            if (!File.Exists(path))
            {
                var notFound = new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Matchup data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                };
                logger.Log(LogTarget.Console, LogType.Error, DateTime.UtcNow, $"{notFound.ErrorMessage}");
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(notFound);
            }
            var json = await File.ReadAllTextAsync(path);
            var matchups = JsonSerializer.Deserialize<List<Matchup>>(json)!
                .Where(m => m.Season.ToString() == season && m.Week.ToString() == week).ToList();

            if (matchups is not null)
            {
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(matchups);
            }
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(standings, options);
                await File.WriteAllTextAsync(path, json);
                return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Ok(standings);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingStandings = JsonSerializer.Deserialize<List<TeamStanding>>(file) ?? [];
                existingStandings.AddRange(standings);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingStandings, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>.Ok(existingStandings);
            }
        }
        #endregion

        #region WRITE MEMBER TO JSON

        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteMemberToJsonAsync(GuildMember member)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(member, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var players = JsonSerializer.Deserialize<List<GuildMember>>(file) ?? [];
                players.Add(member);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(players, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }

        #endregion

        #region GET MEMBER FROM JSON
        public async Task<Result<GuildMember, SystemError<JsonDataServiceProvider>>> GetMemberFromJsonAsync(string memberId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Player data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var file = await File.ReadAllTextAsync(path);
            var members = JsonSerializer.Deserialize<List<GuildMember>>(file) ?? [];
            var member = members.FirstOrDefault(p => p.Id == int.Parse(memberId));
            if (member is not null) return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Ok(member);

            return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch member data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
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
                var players = JsonSerializer.Deserialize<List<PoolPlayer>>(file) ?? [];
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(source, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            var file = await File.ReadAllTextAsync(path);
            var stats = JsonSerializer.Deserialize<List<TeamStats>>(file) ?? [];
           
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
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(stats, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingStats = JsonSerializer.Deserialize<List<TeamStats>>(file) ?? [];
                existingStats.Add(stats);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingStats, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }
        #endregion

        #region WRITE SEASON SCHEDULE FOR TEAM
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteSeasonScheduleToJson(List<Matchup> matchups, string teamName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"{teamName}_schedule_{matchups[0].Season}.json");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(matchups, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "schedule data already exists",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }
        #endregion

        #region GET SEASON SCHEDULE FOR TEAM
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetSeasonScheduleFromJsonAsync(int season, string teamName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"{teamName}_schedule_{season}.json");
            if (!File.Exists(path))
            {
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Schedule data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var schedule = JsonSerializer.Deserialize<List<Matchup>>(json)!;
            if (schedule is not null) return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(schedule);
            return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch schedule data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region GET DRAFT FROM JSON
        public async Task<Result<List<DraftEntity>, SystemError<JsonDataServiceProvider>>> GetDraftFromJsonAsync(int season, string teamName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"{teamName}_{season}_draft.json");
            if (!File.Exists(path))
            {
                return Result<List<DraftEntity>, SystemError <JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "draft data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }

            var json = await File.ReadAllTextAsync(path);
            var drafts = JsonSerializer.Deserialize<List<DraftEntity>>(json)!.Where(x => x.TeamName.Equals(teamName.ToTeamFullName())).ToList();
            if (drafts is not null) return Result<List<DraftEntity>, SystemError<JsonDataServiceProvider>>.Ok(drafts);
            return Result<List<DraftEntity>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch schedule data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region WRITE DRAFT TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteDraftToJsonAsync(List<DraftEntity> source, int season)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"{season}_draftResults.json");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(source, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Draft data already exists.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
        }
        #endregion

    }
}
