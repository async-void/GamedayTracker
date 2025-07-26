using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IGameData
    {
        Result<List<Matchup>, SystemError<GameDataService>> GetCurrentScoreboard();
        Task<Result<List<Matchup>, SystemError<GameDataService>>> GetScoreboard(int season, int week);
        Task<Result<List<Matchup>, SystemError<GameDataService>>> GetTeamSchedule(string teamName);
        Result<int, SystemError<GameDataService>> GetCurWeek();
        Result<int, SystemError<GameDataService>> GetCurSeason();
        int GetMatchupCount(int season, int week);
    }
}
