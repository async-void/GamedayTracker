using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Settings.Moderation
{
    [Command("mod-settings")]
    [Description("moderation settings")]
    public class ModerationSettingsSlashCommands
    {
        [Command("notification-channel")]
        [Description("set the notification channel to receive bot notifications")]
        [RequirePermissions(DiscordPermission.ManageGuild)]
        public async ValueTask SetNotificationChannel(CommandContext ctx, [Description("channel id")] DiscordChannel channel)
        {
            await ctx.DeferResponseAsync();
            await ctx.RespondAsync("Moderation settings set");
        }
    }
}
