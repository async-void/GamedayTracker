using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IGuildMemberService
    {
        Task<Result<GuildMember, SystemError<GuildMemberService>>> GetGuildMemberAsync(ulong guildId, ulong memberId);
        Task<Result<GuildMember, SystemError<GuildMemberService>>> SaveGuildMemberAsync(ulong guildId, ulong memberId);
        Task<Result<GuildMember, SystemError<GuildMemberService>>> RemoveGuildMemberAsync(ulong guildId, ulong memberId);

    }
}
