using GamedayTracker.Models;
using GamedayTracker.Models.NFL.InjuryReport;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IInjuryReport
    {
        Task<Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>> GetTeamInjuryReportAsync(string teamName);
    }
}
