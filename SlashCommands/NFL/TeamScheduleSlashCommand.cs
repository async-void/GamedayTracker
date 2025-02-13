using System.ComponentModel;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.NFL
{
    public class TeamScheduleSlashCommand
    {
        [Command("schedule")]
        [Description("Gets selected Team's Schedule")]
        public async Task GetTeamSchedule(CommandContext cyx, [Parameter("team")] string teamName)
        {

        }
    }
}
