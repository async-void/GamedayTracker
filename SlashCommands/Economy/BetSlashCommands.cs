using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.AutoCompleteProvider;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands(ICommandHelper slashCmdHelper, IJsonDataService jsonService, IGameData gameDataService, IBetting bettingService)
    {
       
        [Command("bet")]
        [Description("place bet on a matchup")]
        public async Task Bet(SlashCommandContext ctx, [Parameter("amount")] int amount, [SlashAutoCompleteProvider<GameDayAutoCompleteProvider>] string day)
        {
            //TODO: finish betting command
            await ctx.DeferResponseAsync();

            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var user = ctx.User;
            var userFromJson = await jsonService.GetMemberFromJsonAsync(user.Id, ctx.Guild?.Id ?? 0);
            if (userFromJson.IsOk)
            {
                if (userFromJson.Value.Bank is { } bank)
                {
                    var canAffordBet = await bettingService.CanAffordBetAsync(bank, amount);
                    if (canAffordBet.IsOk)
                    {
                        var scoreboard = await gameDataService.GetNFLScoresAsync(2025, 1);
                        var scheduled = scoreboard.Events
                            .Where(s => s.Date.DayOfWeek.ToString().Equals(day))
                            .ToList();
                        if (scheduled.Count == 0)
                        {
                            await ctx.RespondAsync("No scheduled games found, unable to place bet at this time.");
                            return;
                        }

                        IEnumerable<DiscordSelectComponentOption> gameOptions = scheduled.Select(s =>
                        {
                            var awayTeam = s.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway.Equals("away"))?.Team.DisplayName ?? "Unknown";
                            var homeTeam = s.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway.Equals("home"))?.Team.DisplayName ?? "Unknown";
                            var optionLabel = $"{awayTeam} beats {homeTeam}";
                            var optionValue = $"{s.Name}";
                            return new DiscordSelectComponentOption(optionLabel, optionValue);
                        });

                        var scheduleData = scheduled[0].ShortName;

                        DiscordComponent[] comps =
                         [
                            new DiscordTextDisplayComponent("### Place Your Bet"),
                            new DiscordSeparatorComponent(true),
                            new DiscordActionRowComponent(
                                [
                                    new DiscordSelectComponent("betting", "Scheduled Games", gameOptions)
                                ]
                            )
                        ];

                        var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);
                        var embed = new DiscordMessageBuilder()
                            .EnableV2Components()
                            .AddContainerComponent(container);
                        await ctx.EditResponseAsync(embed);
                    }
                    else
                    {
                        await ctx.RespondAsync("You do not have enough funds to place this bet.");
                    }
                }
                else
                {
                    await ctx.RespondAsync("You do not have enough funds to place this bet.");
                }
            }
            else
            {
                DiscordComponent[] errComps =
                [
                    new DiscordTextDisplayComponent($"❌ ERROR ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{userFromJson.Error.ErrorMessage}"),
                    new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}")
                ];
                var errContainer = new DiscordContainerComponent(errComps, false, DiscordColor.DarkRed );
                var errMsg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(errContainer);
                await ctx.RespondAsync( errMsg );
            }

            
        }

        #region LEADERBOARD
        [Command("leaderboard")]
        [Description("get the betting leaderboard")]
        public async Task Leaderboard(SlashCommandContext ctx, [SlashChoiceProvider<LeaderboardChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var leaderboard = new Result<List<GuildMember>, SystemError<SlashCommandHelper>>();

            switch (choice)
            {
                case 0:
                    leaderboard = await slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id, choice);
                    break;
                case 1:
                    leaderboard = await slashCmdHelper.BuildLeaderboard(0, choice);
                    break;
                default:
                   
                    return;
            }
            
            var title = choice switch
            {
                0 => "Server Leaderboard",
                1 => "Global Leaderboard",
                _ => "Leaderboard"
            };

            if (!leaderboard.IsOk)
            {
                DiscordComponent[] errComponents =
                [
                    new DiscordTextDisplayComponent("Error"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"{leaderboard.Error.ErrorMessage}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {timestamp}")
                ];
                var errContainer = new DiscordContainerComponent(errComponents, false, DiscordColor.DarkRed);
                var errEmbed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(errContainer);
                await ctx.EditResponseAsync(errEmbed);
                return;
            }

            var embedDesc = slashCmdHelper.BuildLeaderboardDescription(leaderboard.Value).Value;
            
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"**{title}** 🏆"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"{embedDesc}"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {timestamp}")
            ];
            var container = new DiscordContainerComponent(components, false, DiscordColor.Teal);
            var ldbEmbed = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.EditResponseAsync(ldbEmbed);
        }
        #endregion
    }
}
