using GamedayTracker.Models;
using GamedayTracker.Models.NFL;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IGameData
    {
        Task<Result<List<Matchup>, SystemError<GameDataService>>> GetCurrentScoreboard();
        Task<Result<List<Matchup>, SystemError<GameDataService>>> GetScoreboard(int season, int week);
        Task<Result<List<Matchup>, SystemError<GameDataService>>> GetTeamSchedule(string teamName);
        Result<int, SystemError<GameDataService>> GetCurWeek();
        Result<int, SystemError<GameDataService>> GetCurSeason();
        int GetMatchupCount(int season, int week);
        Task<NFLScoreboard> GetNFLScoresAsync(int? season = null, int? week = null, int? seasonType = null);
        string GetLineScores(Competitor away, Competitor home);
        Task<string> GetGameInfo(Event game);
        string GetGameLeaders(Competition competition);
        List<Event> GetScheduledGames(NFLScoreboard scoreboard);
        List<Event> GetCompletedGames(NFLScoreboard scoreboard);
        List<Event> GetLiveGames(NFLScoreboard scoreboard);
        string GetQuarterName(int period);
        string GetGameStatus(Competition competition);
        bool IsGameScheduled(Competition competition);
        bool IsGameCompleted(Competition competition);
        bool IsGameInProgress(Competition competition);
        Task<(string, NFLTeam)> GetTeamRecordAsync(string teamAbbreviation);
        List<Event> GetTeamGames(NFLScoreboard scoreboard, string teamAbbreviation);
        int GetSeasonType(NFLScoreboard scoreboard);
        string GetSeasonTypeName(int seasonType);
        string GetWeekDisplayName(NFLScoreboard scoreboard);
        string GetPlayoffWeekName(NFLScoreboard scoreboard);
        string GetFullSeasonWeekDisplay(NFLScoreboard scoreboard);
        Task<NflStandings> GetNFLStandingsAsync();
        Task<NFLScoreboard> GetEspnTeamScheduleAsync(string teamAbbr, int seasonType, int season);
        Task<NFLScoreboard> GetScoreboardByEventId(string eventId);
    }
}
