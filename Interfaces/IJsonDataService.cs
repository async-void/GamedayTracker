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
        Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> WriteAllMatchupsToJson(List<Matchup> matchups);
    }
}
