using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Helpers
{
    public class SlashCommandHelper(IJsonDataService dataService): ICommandHelper
    {
        private readonly IJsonDataService _dataService = dataService;

        #region BUILDLEADERBOARD
        /// <summary>
        /// Build a leaderboard based on the Guild or Global scope
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="scope"></param>
        /// <returns>a list of guild members</returns>
        public async Task<Result<List<GuildMember>, SystemError<SlashCommandHelper>>> BuildLeaderboard(string guildId, int scope)
        {
            List<GuildMember> leaderboard;

            switch (scope)
            {
                case 0://Guild
                    var leaderboardResult = await _dataService.GetAllMembersForScope(0, guildId);

                    if (leaderboardResult.IsOk)
                    {
                        leaderboard = [.. leaderboardResult.Value
                            .Where(m => m.GuildId == guildId)
                            .OrderByDescending(m => m.BetWins)];
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Ok(leaderboard);
                    }
                    else
                    {
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Err(new SystemError<SlashCommandHelper>
                        {
                            ErrorMessage = leaderboardResult.Error.ErrorMessage,
                            ErrorType = leaderboardResult.Error.ErrorType,
                            CreatedAt = DateTime.Now,
                            CreatedBy = this
                        });
                    }

                case 1://Global
                    leaderboardResult = await _dataService.GetAllMembersForScope(1);

                    if (leaderboardResult.IsOk)
                    {
                        leaderboard = [.. leaderboardResult.Value
                            .OrderByDescending(m => m.BetWins)];
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Ok(leaderboard);
                    }
                    else
                    {
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Err(new SystemError<SlashCommandHelper>
                        {
                            ErrorMessage = leaderboardResult.Error.ErrorMessage,
                            ErrorType = leaderboardResult.Error.ErrorType,
                            CreatedAt = DateTime.Now,
                            CreatedBy = this
                        });
                    }

                default:
                    return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Err(new SystemError<SlashCommandHelper>()
                    {
                        ErrorMessage = "Unknown Error",
                        ErrorType = ErrorType.FATAL,
                        CreatedAt = DateTime.Now,
                        CreatedBy = this
                    });

            }
        }
        #endregion

        #region BUILD LEADERBOARD DESCRIPTION
        public Result<string, SystemError<SlashCommandHelper>> BuildLeaderboardDescription(List<GuildMember> members)
        {
            var builder = new StringBuilder(); 
            var idx = 1;
            const string prefix = "#";
            const string uName = "Username";
            const string balance = "Balance";
            builder.Append($"``{prefix.PadRight(2)} {uName} {balance.PadLeft(10)}``\r\n");

            foreach (var member in members)
            {
                builder.Append($"``{idx.ToString(CultureInfo.CurrentCulture)}``. {member.MemberName.PadLeft(8)} {member.Balance}");
                idx++;
            }

            return Result<string, SystemError<SlashCommandHelper>>.Ok(builder.ToString());
        }
        #endregion

        #region BUILD HELP SECTION
        public Result<List<Page>, SystemError<SlashCommandHelper>> BuildHelpSection()
        {
            var builder = new StringBuilder();
            builder.AppendLine("**Gameday Tracker Slash Commands**\r\n");
            builder.AppendLine("## Utility Commands");
            builder.AppendLine("`/ping` - get Discord's and the DB's latency");
            builder.AppendLine("`/help` - Show this help message");
            builder.AppendLine("`/leaderboard` - Show the leaderboard for the current guild or globally");
            builder.AppendLine("`/profile` - View your profile");
            builder.AppendLine("`/deposit` - Deposit money into your account");
            builder.AppendLine("`/withdraw` - Withdraw money from your account");
            builder.AppendLine("`/balance` - Check your balance");
            builder.AppendLine("`/donate` - Donate to the bot");
            var pages = new List<Page>
            {
                new() { Content = builder.ToString(), },
                
            };
            return Result<List<Page>, SystemError<SlashCommandHelper>>.Ok(pages);
        }
        #endregion
    }
}
