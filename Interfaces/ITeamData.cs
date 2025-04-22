using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface ITeamData
    {
        Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultsAsync(int year);
        Task <Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultForTeamAsync(int year, string tName);
        Result<string, SystemError<TeamDataService>> GetTeamNameFromInt(int input);
        Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForAfc();
        Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForNfc();
        bool IsValidTeamName(string name);
    }
}
