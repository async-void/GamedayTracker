﻿using System;
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
        Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultsAsync(string teamName, int year);
        Task <Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultForTeamAsync(int year, string tName);
        Task<Result<List<TeamStats>, SystemError<TeamDataService>>> GetStatsAsync(int choice, int season);
        Task<Result<TeamStats, SystemError<TeamDataService>>> GetTeamStatsAsync(int choice, int season, string teamName);
        Task<Result<List<TeamStanding>, SystemError<TeamDataService>>> GetAllTeamStandings(int season);
        Result<string, SystemError<TeamDataService>> GetTeamNameFromInt(int input);
        Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForAfc();
        Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForNfc();
        Result<List<DiscordSelectComponentOption>, SystemError<TeamDataService>> BuildSelectOptionForUserPicks();
        bool IsValidTeamName(string name);
    }
}
