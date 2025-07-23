using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Checks
{
    public class RequireRoleCheck : IContextCheck<RequireRoleAttribute>
    {
        public async ValueTask<string?> ExecuteCheckAsync(RequireRoleAttribute attribute, CommandContext context)
        {
            DiscordGuild? guild = context.Guild;
            if (guild! == null!)
                return "Command has to be executed in a guild!"; // Return error, if command was executed in dm (if possible)

            DiscordRole requiredRole = await guild.GetRoleAsync(attribute.RequiredRoleId);
            if (!context.Member!.Roles.Contains(requiredRole))
                return $"You'll need the **{requiredRole.Name}** role to use this command!"; // Return error if user doesn't have role

            return null;
        }
    }
}
