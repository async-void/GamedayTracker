using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using GamedayTracker.Helpers;
using GamedayTracker.Services;

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
            var normalizedName = NflTeamMatcher.MatchTeam(teamName) ?? "UNKNOWN";
            var injuryEndpoint = InjuryReportEndpointProviderService.GetTeamInjuryReport(normalizedName);

            await ctx.RespondAsync($"Injury Report is a wip, the bot dev's are hard at work building this feature!\r\n{injuryEndpoint}");
        }
    }
}
