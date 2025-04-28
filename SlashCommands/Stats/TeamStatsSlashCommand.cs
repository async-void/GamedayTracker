using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand
    {
        [Command("teamstats")]
        [Description("Get the Team Statistics")]
        public async Task GetTeamStats(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
        }
    }
}
