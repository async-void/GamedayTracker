using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IGameData
    {
        Result<List<Matchup>, SystemError<GameDataService>> GetScoreboard(int season, int week);
        Task<Result<List<string>, SystemError<GameDataService>>> GetTeamSchedule(string teamName, int season);
        string GetCurWeek();
        string GetMatchupCount(int season, int week);
    }
}
