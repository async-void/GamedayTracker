using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.SlashCommands.Development
{
    public class KnownIssuesCommand
    {
        [Command("bug-issues")]
        [Description("List known bugs/issues GamedayTracker")]
        public static async Task Execute(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var issues = new StringBuilder();
            issues.AppendLine("1. ~~**Bot Status Update**: The bot status update job may not reflect the correct number of servers or users at times.~~");
            issues.AppendLine("2. **Daily Numbers Job**: The daily numbers job is not functioning as expected. It may not send messages to the default channel in some servers.");
            issues.AppendLine("3. **Betting**: in game betting is being work on.");

            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent("### Known Issues with GamedayTracker 🪲"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent(issues.ToString()),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# GamedayTracker ©️ {unixTimestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),

            ];
            var container = new DiscordContainerComponent(comps, false, DiscordColor.DarkGray);
            await ctx.RespondAsync(new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container));
        }
    }
}
