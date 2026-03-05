using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Cache;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Pagination;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand(ITeamData teamDataService, IDiscordEmbedService embedService)
    {
        [Command("teamstats")]
        [Description("Get Team Stats")]
        public async Task GetTeamStats(SlashCommandContext ctx, NFLSeasonType seasonChoice, 
            [Parameter("team"), Description("example: Buffalo or Pittsburgh")] string teamName, [SlashChoiceProvider<SeasonChoiceProvider>] int season)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();

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
              
                var teamDetail = normalizedName?.Split(" ")[0] ?? normalizedName;
                var abbr = teamDetail?.ToAbbr() ?? NflEmojiService.GetEmoji("default");
                var teamEmoji = NflEmojiService.GetEmoji(abbr);
                var msg = await embedService.CreateTeamStatsPage(stats.Value,teamEmoji, seasonChoice, season, 0);

                var buttons = PaginationBuilder.CreateNavigationButtons(0, stats.Value.Splits.Categories.Count);
                msg.AddActionRowComponent(new DiscordActionRowComponent(buttons));
                //var embed = await embedService.CreateTeamStatsEmbed(stats.Value, seasonChoice, season, abbr);
                await ctx.RespondAsync(msg);

                var response = await ctx.GetResponseAsync();

                var paginationData = new TeamStatsPaginationData
                {
                    TeamStats = stats.Value,
                    Emoji = teamEmoji,
                    SeasonType = seasonChoice,
                    Season = season,
                    CurrentPage = 0,
                    TotalPages = stats.Value.Splits.Categories.Count,
                    UserId = ctx.User.Id,
                    MessageId = response.Id
                };

                PaginationCache.Store(response.Id, paginationData);

                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMinutes(10));
                    TeamStatsPaginationCache.Remove(response.Id);
                });
            }
        }
    }
 }
