using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GamedayTracker.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Checks
{
    public class RequireRoleCheck(ulong roleId) : IContextCheck
    {
        private readonly ulong _roleId = roleId;

        public async ValueTask<bool> ExecuteCheckAsync(CommandContext ctx)
        {
            if (ctx.Guild is null)
                return false;

            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);

            return member.Roles.Any(r => r.Id == _roleId);
        }

    }
}
