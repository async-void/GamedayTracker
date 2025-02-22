using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.Notifications
{
    public class NotificationSlashCommands
    {
        [Command("notify")]
        [Description("sends a notification to each guild the bot is in")]
        public async ValueTask Notify(CommandContext ctx, [Description("guild to notify")] ulong guildId)
        {
            await ctx.DeferResponseAsync();
            var guild = await ctx.Client.GetGuildAsync(guildId);
            var message = new StringBuilder();
           
            await ctx.EditResponseAsync(message.ToString());
        }
    }
}
