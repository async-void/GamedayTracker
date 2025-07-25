using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GamedayTracker.Services
{
    public class JsonDataServiceProvider() : IJsonDataService
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
                Serilog.Log.Information($"{notFound.ErrorMessage}");
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            List<GuildMember> members;

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                members = [member];
                var json = JsonSerializer.Serialize(members, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                members = JsonSerializer.Deserialize<List<GuildMember>>(file) ?? [];
                var curMember = members.FirstOrDefault(m => m.GuildId.Equals(member.GuildId) && m.MemberId.Equals(member.MemberId));
                if (curMember is null)
                    members.Add(member);
            }
            var updatedJson = JsonSerializer.Serialize(members, options);
            await File.WriteAllTextAsync(path, updatedJson);
            return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);

        }

        #endregion

        #region GET ALL MEMBERS FOR SCOPE
        public async Task<Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>> GetAllMembersForScope(int scope, string? guildId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Player data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }

            switch (scope)
            {
                case 0: //Guild Scope
                    if (guildId is not null)
                    {
                        var file = await File.ReadAllTextAsync(path);
                        var members = JsonSerializer.Deserialize<List<GuildMember>>(file) ?? [];
                        var guildMembers = members
                            .Where(m => m.GuildId.ToString().Equals(guildId, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        if (guildMembers.Count > 0)
                        {
                            return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Ok(guildMembers);
                        }
                        return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                        {
                            ErrorMessage = "No members found for the specified guild.",
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.INFORMATION,
                            CreatedBy = this
                        });
                    }
                    else
                    {
                        return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                        {
                            ErrorMessage = "Guild Id not found or Guild Id was not provided.",
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.INFORMATION,
                            CreatedBy = this
                        });
                    }

                case 1://Global Scope
                    var _file = await File.ReadAllTextAsync(path);
                    var _members = JsonSerializer.Deserialize<List<GuildMember>>(_file) ?? [];
                    if (_members.Count > 0)
                    {
                        return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Ok(_members);
                    }

                    return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                    {
                        ErrorMessage = "No members found.",
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION,
                        CreatedBy = this
                    });

                default:
                    break;
            }
            return Result<List<GuildMember>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Unkown error occured...unable to get members list",
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region GET MEMBER FROM JSON
        public async Task<Result<GuildMember, SystemError<JsonDataServiceProvider>>> GetMemberFromJsonAsync(string memberId, string guildId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Player data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var file = await File.ReadAllTextAsync(path);
            var members = JsonSerializer.Deserialize<List<GuildMember>>(file);
            var member = members!
                .Where(p => p.MemberId.Equals(memberId) && p.GuildId.Equals(guildId))
                .FirstOrDefault();
            if (member is { } mem) return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Ok(mem);

            //member is not found
            return Result<GuildMember, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Member data not found.",
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region UPDATE MEMBER DATA 
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> UpdateMemberDataAsync(GuildMember member)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Member data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = File.ReadAllText(path);
            JsonArray? members = JsonNode.Parse(json)?.AsArray();
            if (members is not null)
            {
                var mem = members.OfType<JsonObject>()
                    .FirstOrDefault(m => m["MemberId"]!.ToString().Equals(member.MemberId.ToString()) && m["GuildId"]!.ToString().Equals(member.GuildId.ToString()));

                if (mem is not null)
                {
                    mem["FavoriteTeam"] = member.FavoriteTeam;

                    await File.WriteAllTextAsync(path, members.ToJsonString(new JsonSerializerOptions {  WriteIndented = true}));
                    return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
                }
            }
            return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Member data not found.",
                CreatedAt = DateTimeOffset.UtcNow,
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
                //TODO: if file does not exist, call TeamDataService.GetTeamStatsAsync() to fetch from API and write to file
                
                return Result<TeamStats, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Team stats data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var lineType = choice == 0 ? LineType.Offense : LineType.Defense;
           
            var stats = JsonSerializer.Deserialize<List<TeamStats>>(json)!
                .Where(x => x.Season == season && x.LineType.Equals(lineType) && x.TeamName!.Equals(teamName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (stats is not null) return Result<TeamStats, SystemError<JsonDataServiceProvider>>.Ok(stats);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch team stats data.",
                CreatedAt = DateTimeOffset.UtcNow,
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
                    CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
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
                    CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
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
                    CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
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
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
        }
        #endregion

        #region GET MEMBER GUILD FROM JSON

        public async Task<Result<Guild, SystemError<JsonDataServiceProvider>>> GetMemberGuildFromJsonAsync(string memberId, string guildId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");
            if (!File.Exists(path))
            {
                return Result<Guild, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Member data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var file = await File.ReadAllTextAsync(path);
            var members = JsonSerializer.Deserialize<List<GuildMember>>(file);
            var member = members!
                .Where(p => p.MemberId.ToString().Equals(memberId))
                .FirstOrDefault();
            //member is not found
            return Result<Guild, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Member guild data not found.",
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }

        #endregion

        #region GET ALL GUILDS FROM JSON
        public async Task<Result<List<Guild>, SystemError<JsonDataServiceProvider>>> GetGuildsFromJsonAsync()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "guilds.json");
            if (!File.Exists(path))
            {
                return Result<List<Guild>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Guild data file not found.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var guilds = JsonSerializer.Deserialize<List<Guild>>(json)!;
            if (guilds is { } glds) return Result<List<Guild>, SystemError<JsonDataServiceProvider>>.Ok(glds);
            return Result<List<Guild>, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch guild data.",
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region GET GUILD FROM JSON
        public async Task<Result<Guild, SystemError<JsonDataServiceProvider>>> GetGuildFromJsonAsync(string guildId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "guilds.json");
            if (!File.Exists(path))
            {
                return Result<Guild, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("2a35e0b1-3f87-4c7a-8e6d-5bc1cf301b1f")),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            var guilds = JsonSerializer.Deserialize<List<Guild>>(json)!;
            var guild = guilds.FirstOrDefault(g => g.GuildId.Equals(guildId, StringComparison.OrdinalIgnoreCase));
            if (guild is { }) return Result<Guild, SystemError<JsonDataServiceProvider>>.Ok(guild);

            return Result<Guild, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("2a35e0b1-3f87-4c7a-8e6d-5bc1cf301b1f")),
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region WRITE GUILD TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteGuildToJsonAsync(Guild guild)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "guilds.json");
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(new List<Guild> { guild }, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingGuilds = JsonSerializer.Deserialize<List<Guild>>(file) ?? [];
                var guildExists = existingGuilds.Any(g => g.GuildId.Equals(guild.GuildId));

                if (!guildExists)
                {
                    existingGuilds.Add(guild);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var updatedJson = JsonSerializer.Serialize(existingGuilds, options);
                    await File.WriteAllTextAsync(path, updatedJson);
                    return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
                }
                return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Guild data already exists.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
        }
        #endregion

        #region UPDATE GUILD DATA
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> UpdateGuildDataAsync(Guild guild)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "guilds.json");
            if (!File.Exists(path))
            {
                return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("f416e176-85b0-4f94-b172-8dc8f084242e")),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            List<Guild> guilds = JsonSerializer.Deserialize<List<Guild>>(json) ?? [];

            var guildToUpdate = guilds.FirstOrDefault(g => g.GuildId.Equals(guild.GuildId.ToString()));
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            if (guildToUpdate is { })
            {
                guilds.Remove(guildToUpdate);
                guilds.Add(guild);
                json = JsonSerializer.Serialize(guilds, jsonOptions);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }

            return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("2a35e0b1-3f87-4c7a-8e6d-5bc1cf301b1f")),
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion

        #region UPDATE GUILD DATA
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> RemoveGuildDataAsync(string guildId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "guilds.json");
            if (!File.Exists(path))
            {
                return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("f416e176-85b0-4f94-b172-8dc8f084242e")),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                });
            }
            var json = await File.ReadAllTextAsync(path);
            List<Guild> guilds = JsonSerializer.Deserialize<List<Guild>>(json) ?? [];

            var guildToUpdate = guilds.FirstOrDefault(g => g.GuildId.Equals(guildId));
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            if (guildToUpdate is { })
            {
                guilds.Remove(guildToUpdate);
                json = JsonSerializer.Serialize(guilds, jsonOptions);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }

            return Result<bool, SystemError<JsonDataServiceProvider>>.Err(new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("2a35e0b1-3f87-4c7a-8e6d-5bc1cf301b1f")),
                CreatedAt = DateTimeOffset.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            });
        }
        #endregion
    }
}
