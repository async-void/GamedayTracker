using DSharpPlus.SlashCommands;

namespace GamedayTracker.Attributes
{
    public class RequireRoleAttribute(ulong[] roles) : SlashCheckBaseAttribute
    {
        private readonly ulong[] Roles = roles;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            var memRoles = ctx.Member.Roles.Select(role => role.Id).ToArray();
            foreach (var role in Roles)
            {
                if (memRoles.Contains(role))
                {
                    return true; 
                }
            }
            return false;
        }
    }
}
