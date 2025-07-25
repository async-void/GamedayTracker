using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.SlashCommands.Feeds
{
    public class LiveFeeds
    {
        [Command("live-feeds")]
        [Description("Get live feeds for Scores, Standings and Daily Headlines.")]
        public async ValueTask GetLiveFeeds(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent("## Live Feeds\r\n-# live feeds for Scores, Standings and Headlines"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent("choose the desired feed and `Follow` to receive live feed."),
                new DiscordSeparatorComponent(true),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"Live Scores"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "scoresId", "Live Scores")),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Daily Headlines"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "headlinesId", "Headlines")),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            ];
            var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            var message = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            await ctx.EditResponseAsync(message);
        }
    }
}
