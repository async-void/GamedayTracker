using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands
{
    public class TeamScheduleSlashCommand
    {
        [DSharpPlus.Commands.Command("schedule")]
        [Description("Gets selected Team's Schedule")]
        public async Task GetTeamSchedule(CommandContext cyx, [Option("team name", "Team to pull schedule for")] string teamName)
        {

        }
    }
}
