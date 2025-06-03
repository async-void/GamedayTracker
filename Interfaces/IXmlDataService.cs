using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IXmlDataService
    {
        Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteAllMatchupsToXmlAsync(List<Matchup> matchups, string season);
        Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteMatchupAsync(Matchup matchup);
        Task<Result<List<Matchup>, SystemError<XmlDataServiceProvider>>> GetSeasonMatchupDataAsync(string season);
        Task<Result<List<Matchup>, SystemError<XmlDataServiceProvider>>> GetWeekMatchupDataAsync(string season, string week);
        Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteSeasonStandingsToXmlAsync(List<TeamStanding> standings, int season);
        Task<Result<List<TeamStanding>, SystemError<XmlDataServiceProvider>>> GetSeasonStandingsFromXmlAsync(int season);
    }
}
