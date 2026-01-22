using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand(ITeamData teamDataService, IDiscordEmbedService embedService)
    {
        [Command("teamstats")]
        [Description("Get [Offense, Defense] Stats")]
        public async Task GetTeamStats(SlashCommandContext ctx, NFLSeasonType seasonChoice, 
            [Parameter("team"), Description("example: Buffalo or Pittsburgh")] string teamName, [SlashChoiceProvider<SeasonChoiceProvider>] int season)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var seasonType = 0;

            var normalizedName = NflTeamMatcher.MatchTeam(teamName) ?? teamName;
            var stats = await teamDataService.GetTeamStatsAsync(seasonChoice, season, normalizedName);
            
            if (!stats.IsOk)
            {
                DiscordComponent[] errComps =
                [
                    new DiscordTextDisplayComponent($"❌ ERROR ❌"),
                    new DiscordSeparatorComponent(),
                    new DiscordTextDisplayComponent(stats.Error.ErrorMessage!),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"Gameday Tracker {unixTimestamp}"),
                ];
                var errContainer = new DiscordContainerComponent(errComps, false, DiscordColor.Red);
                var errMsg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(errContainer);
                await ctx.RespondAsync(errMsg);
            }
            else
            {
                var embed = await embedService.CreateTeamStatsEmbed(stats.Value, seasonChoice, season);
                await ctx.RespondAsync(embed);
            }
        }
    }
}
