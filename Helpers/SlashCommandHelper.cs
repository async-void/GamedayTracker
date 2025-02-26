using System;
using System.Collections.Generic;
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
    }
}
