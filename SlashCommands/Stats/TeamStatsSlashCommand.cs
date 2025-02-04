using System.ComponentModel;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand
    {
        [Command("teamstats")]
        [Description("Get the Team Statistics")]
        public async Task GetTeamStats(CommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
        }
    }
}
