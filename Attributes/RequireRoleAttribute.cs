using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Log = Serilog.Log;

namespace GamedayTracker.Attributes
{
    public class RequireRoleAttribute(string[] roles) : CheckBaseAttribute
    {
        private readonly string[] _roles = roles;
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var userRoles = ctx.Member.Roles.Select(r => r.Name).ToList();
            Log.Information($"Checking roles..... {_roles}");
            return _roles.Any(role => role.Equals(userRoles.Any()))
                ? Task.FromResult(true)
                : Task.FromResult(false);
        }
    }
}
