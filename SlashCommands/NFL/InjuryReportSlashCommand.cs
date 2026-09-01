using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.NFL
{
    public class InjuryReportSlashCommand(IInjuryReport injuryReportService, IEvaluator seasonEvaluator, ITeamData teamDataService)
    {
        [Command("injury-report")]
        [Description("Get the injury report for a specific team.")]
        public async ValueTask InjuryReport(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var normalizedName = NflTeamMatcher.MatchTeam(teamName) ?? "UNKNOWN";

            var injuries = await teamDataService.GetTeamInjuriesAsync(teamName);
            var season = seasonEvaluator.Evaluate(DateTime.UtcNow);

            if (season.Equals(RealTimeScoresMode.Offseason))
            {
                DiscordComponent[] errComps =
                [
                    new DiscordTextDisplayComponent($"INJURY REPORT: {normalizedName}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent("Injury reports are only available during the regular season"),
                    new DiscordTextDisplayComponent($"SEASON: {season}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {timestamp}")
                ];
                var container = new DiscordContainerComponent(errComps, false, DiscordColor.Blurple);
                var errMsg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.EditResponseAsync(errMsg);
                return;
            }
           
            var injuryEndpoint = InjuryReportEndpointProviderService.GetTeamInjuryReportEndpoint(normalizedName);
            if (injuryEndpoint == null)
            {
                await ctx.RespondAsync($"could not normalize the team name, try again. example KC, Steelers, Buf");
                return;
            }
            else
            {
                //injuries = await injuryReportService.GetTeamInjuryReportAsync(injuryEndpoint);
                //if (injuries.IsOk || injuries.Value.Count <= 0)
                //{

                //}
                //else
                    await ctx.RespondAsync($"Injury Report is a wip, the bot dev's are hard at work building this feature!\r\n{normalizedName}\r\n{injuryEndpoint}");
            }
           
        }
    }
}
