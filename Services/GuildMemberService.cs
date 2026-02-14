using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private readonly string _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "members.json");

        #region GET GUILD MEMBER
        public async Task<Result<GuildMember, SystemError<GuildMemberService>>> GetGuildMemberAsync(ulong guildId, ulong memberId)
        {
            if (File.Exists(_jsonPath))
            {
                var json = await File.ReadAllTextAsync(_jsonPath);
                var members = JsonSerializer.Deserialize<List<GuildMember>>(json);
                var member = members?.Where(x => x.MemberId.Equals(memberId) && x.GuildId.Equals(guildId));

                if (member is not null) 
                    return Result<GuildMember, SystemError<GuildMemberService>>.Ok(member.First());

                return Result<GuildMember, SystemError<GuildMemberService>>.Err(new SystemError<GuildMemberService>
                {
                    ErrorMessage = "Member data not found.",
                    ErrorCode = Guid.NewGuid(),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this,
                    CreatedAt = DateTimeOffset.Now,
                });
            }
            else
            {
                return Result<GuildMember, SystemError<GuildMemberService>>.Err(new SystemError<GuildMemberService>
                {
                    ErrorMessage = "Member data not found.",
                    ErrorCode = Guid.NewGuid(),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this,
                    CreatedAt = DateTimeOffset.Now,
                });
            }
        }

        #endregion

        #region REMOVE GUILD MEMBER
        public Task<Result<GuildMember, SystemError<GuildMemberService>>> RemoveGuildMemberAsync(ulong guildId, ulong memberId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SAVE GUILD MEMBER

        public Task<Result<GuildMember, SystemError<GuildMemberService>>> SaveGuildMemberAsync(ulong guildId, ulong memberId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
