using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IJsonDataService
    {
        Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetMatchupsAsync(string season, string week);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteMatchupToJson(Matchup matchup);
        Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> WriteAllMatchupsToJson(List<Matchup> matchups, int season);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteMemberToJsonAsync(GuildMember member);
        Task<Result<GuildMember, SystemError<JsonDataServiceProvider>>> GetMemberFromJsonAsync(string playerId);
        Task<Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>> WriteStandingsToJsonAsync(List<TeamStanding> standings, int season);
        Task<Result<List<TeamStanding>, SystemError<JsonDataServiceProvider>>> GetStandingsFromJsonAsync(int season);
        Task<Result<int, SystemError<JsonDataServiceProvider>>> GeneratePlayerIdAsync();
        Result<ulong, SystemError<JsonDataServiceProvider>> GeneratePlayerIdentifier();
        Task<Result<TeamStats, SystemError<JsonDataServiceProvider>>> GetTeamStatsFromJsonAsync(int choice, int season, string teamName);
        Task<Result<List<TeamStats>, SystemError<JsonDataServiceProvider>>> GetAllTeamStatsFromJsonAsync(int choice, int season, string teamName);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteTeamStatsToJsonAsync(TeamStats stats);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteAllTeamStatsToJsonAsync(List<TeamStats> stats);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteSeasonScheduleToJson(List<Matchup> matchups, string teamName);
        Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetSeasonScheduleFromJsonAsync(int season, string teamName);
        Task<Result<List<DraftEntity>, SystemError<JsonDataServiceProvider>>> GetDraftFromJsonAsync(int season, string teamName);
        Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteDraftToJsonAsync(List<DraftEntity> source, int season);
    }
}
