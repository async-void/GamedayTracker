using DSharpPlus.SlashCommands;

namespace GamedayTracker.Attributes
{
    public class RequireRoleAttribute(string[] roles) : SlashCheckBaseAttribute
    {
        private readonly string[] Roles = roles;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            var memRoles = ctx.Member.Roles.Select(role => role.Name).ToArray();
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
