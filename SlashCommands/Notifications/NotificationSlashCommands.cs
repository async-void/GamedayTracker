using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Notifications
{
    public class NotificationSlashCommands
    {
        [Command("notify")]
        [Description("sends a notification to each guild the bot is in")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async ValueTask Notify(CommandContext ctx, [System.ComponentModel.Description("guild to notify")] ulong guildId)
        {
            await ctx.DeferResponseAsync();
            var guild = await ctx.Client.GetGuildAsync(guildId);
            var message = new StringBuilder();
           
            await ctx.EditResponseAsync(message.ToString());
        }
    }
}
