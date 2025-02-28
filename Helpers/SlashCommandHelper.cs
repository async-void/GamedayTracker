using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Helpers
{
    public class SlashCommandHelper : ICommandHelper
    {
        #region BUILDLEADERBOARD
        public Result<List<GuildMember>, SystemError<SlashCommandHelper>> BuildLeaderboard(string guildId, int scope)
        {
            var leaderboard = new List<GuildMember>();
            var db = new BotDbContextFactory().CreateDbContext();

            switch (scope)
            {
                case 0:
                    leaderboard = db.Members
                        .Include(x => x.Bank)
                        .OrderBy(x => x.Bank!.Balance)
                        .ToList();
                    if (leaderboard.Count > 0)
                    {
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Ok(leaderboard);
                    }
                    else
                    {
                       return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Err(new SystemError<SlashCommandHelper>()
                       {
                           ErrorMessage = "No members found",
                           ErrorType = ErrorType.INFORMATION,
                           CreatedAt = DateTime.Now,
                           CreatedBy = this
                       });
                    }

                case 1:
                    leaderboard = db.Members.Where(x => x.GuildId == guildId)
                        .Include(x => x.Bank)
                        .OrderBy(x => x.Bank!.Balance)
                        .ToList();
                    if (leaderboard.Count > 0)
                    {
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Ok(leaderboard);
                    }
                    else
                    {
                        return Result<List<GuildMember>, SystemError<SlashCommandHelper>>.Err(new SystemError<SlashCommandHelper>()
                        {
                            ErrorMessage = "No members found",
                            ErrorType = ErrorType.INFORMATION,
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
                builder.Append($"``{idx.ToString(CultureInfo.CurrentCulture)}. {member.MemberName.PadLeft(8)} {member.Bank!.Balance.ToString(CultureInfo.CurrentCulture).PadLeft(4)}``\r\n");
                idx++;
            }

            return Result<string, SystemError<SlashCommandHelper>>.Ok(builder.ToString());
        }
        #endregion
    }
}
