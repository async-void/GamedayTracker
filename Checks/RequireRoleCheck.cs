using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

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
