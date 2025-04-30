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

namespace GamedayTracker.Services
{
    public class GuildMemberService : IGuildMemberService
    {
        public async Task<Result<GuildMember, SystemError<GuildMemberService>>> GetGuildMemberAsync(string guildId, string userName)
        {
            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbUser = await db.Members.Where(x => x.MemberName.Equals(userName) &&
                                               x.GuildId == guildId)!
                                                .Include(x => x.Bank)
                                                .Include(x => x.PlayerPicks)
                                                .FirstOrDefaultAsync();

            if (dbUser is null)
            {
                return Result<GuildMember, SystemError<GuildMemberService>>.Err(new SystemError<GuildMemberService>
                {
                    ErrorMessage = "User not found",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this,
                });
            }

            return Result<GuildMember, SystemError<GuildMemberService>>.Ok(dbUser);
        }
    }
}
