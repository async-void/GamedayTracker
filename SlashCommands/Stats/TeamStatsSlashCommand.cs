using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand
    {
        [Command("teamstats")]
        [Description("Get the Team Statistics")]
        public async Task GetTeamStats(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            DiscordComponent[] components =
            {
                new DiscordTextDisplayComponent($"Statistics for Team: **{teamName}**"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent("Team Stats are a wip (work in progress), the devs are working on this feature!"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}")
            };

            var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
            var msgBuilder = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.RespondAsync(msgBuilder);
        }
    }
}
