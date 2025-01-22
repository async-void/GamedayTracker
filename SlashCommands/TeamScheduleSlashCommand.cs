using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands
{
    public class TeamScheduleSlashCommand
    {
        [SlashCommand("schedule", "Gets selected Team's Schedule")]
        public async Task GetTeamSchedule(InteractionContext cyx, [Option("team name", "Team to pull schedule for")] string teamName)
        {

        }
    }
}
