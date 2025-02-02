using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand
    {
        [Command("teamstats")]
        [Description("Get the Team Statistics")]
        public async Task GetTeamStats(CommandContext ctx, [Option("team", "Team Name")] string teamName)
        {

        }
    }
}
