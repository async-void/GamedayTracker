using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand: ApplicationCommandModule
    {
        [SlashCommand("teamstats", "Get the Team Statistics")]
        public async Task GetTeamStats(InteractionContext ctx, [Option("team", "Team Name")] string teamName)
        {

        }
    }
}
