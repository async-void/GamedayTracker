using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace GamedayTracker.SlashCommands.NFL
{
    public class InjuryReportSlashCommand
    {
        [Command("injury-report")]
        [Description("Get the injury report for a specific team.")]
        public async ValueTask InjuryReport(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            //https://www.footballdb.com/teams/nfl/team-name/injuries

            await ctx.RespondAsync("Injury Report is a wip, the bot dev's are hard at work building this feature!");
        }
    }
}
