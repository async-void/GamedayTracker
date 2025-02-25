using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Settings
{
    [Command("settings")]
    [Description("set server settings")]
    public class SettingsSlashCommands
    {
        [Command("set-notifications")]
        [Description("set's the server notifications flag")]
        public async Task SetServerNotifications(CommandContext ctx, bool setFlag)
        {
            await ctx.DeferResponseAsync();
        }
    }
}
