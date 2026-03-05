using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.AutoCompleteProvider;
using GamedayTracker.Cache;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Pagination;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands(ICommandHelper slashCmdHelper, IJsonDataService jsonService, IGameData gameDataService, 
        IBetting bettingService, IDiscordEmbedService embedService)
    {

        #region BET
        [Command("bet")]
        [Description("place bet on a matchup")]
        public async Task Bet(SlashCommandContext ctx, [SlashAutoCompleteProvider<GameDayAutoCompleteProvider>] string day, [Parameter("amount")] int amount)
        {
            //TODO: finish betting command
            await ctx.DeferResponseAsync();

            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var user = ctx.User;
            if (user is not null)
            {
                var userFromJson = await jsonService.GetMemberFromJsonAsync(user.Id, ctx.Guild?.Id ?? 0);
              
                if (userFromJson.IsOk && userFromJson.Value.Bank is { } bank)
                {
                    var canAffordBet = await bettingService.CanAffordBetAsync(bank, amount);
                    if (canAffordBet.IsOk)
                    {
                        var scoreboard = await gameDataService.GetNFLScoresAsync();
                        var scheduled = scoreboard.Events
                            .Where(s => s.Date.DayOfWeek.ToString().Equals(day))
                            .ToList();
                        if (scheduled.Count == 0)
                        {
                            var result = await embedService.BuildErrorContainer(ctx.Client, "No scheduled games found, unable to place bet at this time.", ctx.Guild.Id, DiscordColor.DarkRed);
                            await ctx.EditResponseAsync(new DiscordMessageBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(result));
                            return;
                        }

                        IEnumerable<DiscordSelectComponentOption> gameOptions = scheduled.Select(s =>
                        {
                            var optionLabel = $"{s.Name}";
                            var optionValue = $"{s.Name}:{s.Id}:{amount}";
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
                        var result = await embedService.BuildErrorContainer(ctx.Client, "insufficient funds", ctx.Guild.Id, DiscordColor.DarkRed);
                        var errEmbed = new DiscordMessageBuilder()
                            .EnableV2Components()
                            .AddContainerComponent(result);
                        await ctx.EditResponseAsync(errEmbed);
                        await ctx.RespondAsync("You do not have enough funds to place this bet.");
                    }
                }
                else
                {
                    //user isnt saved in the json file
                    var result = await embedService.BuildErrorContainer(ctx.Client, $"{userFromJson.Error.ErrorMessage}", ctx.Guild.Id, DiscordColor.DarkRed);
                        var errEmbed = new DiscordMessageBuilder()
                            .EnableV2Components()
                            .AddContainerComponent(result);
                        await ctx.EditResponseAsync(errEmbed);
                    
                }
            }
            else
            {
                DiscordComponent[] errComps =
                [
                    new DiscordTextDisplayComponent($"❌ ERROR ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"user not found"),
                    new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}")
                ];
                var errContainer = new DiscordContainerComponent(errComps, false, DiscordColor.DarkRed );
                var errMsg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(errContainer);
                await ctx.RespondAsync( errMsg );
            } 
        }
        #endregion

        #region LIST BETS
        [Command("bets")]
        [Description("list your current bets")]
        public async Task ListBets(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var member = ctx.Member;
            if (member is not null)
            {
                var bets = await bettingService.GetMemberBetsByIdAsync(member.Id, ctx.Guild?.Id ?? 0);
                if (bets.IsOk && bets.Value.Count > 0)
                {
                    var page = await embedService.CreateMemberBetsPage(bets.Value, ctx.Client, 0);
                    var totalPages = (int)Math.Ceiling(bets.Value.Count() / 4.0);
                    var buttons = PaginationBuilder.CreateNavigationButtons(0, totalPages);
                    page.AddActionRowComponent(new DiscordActionRowComponent(buttons));
                    await ctx.RespondAsync(page);
                    var response = await ctx.GetResponseAsync();
                    var paginationData = new MemberBetsPaginationData
                    {
                        UserId = member.Id,
                        DiscordClient = ctx.Client,
                        CurrentPage = 0,
                        TotalPages = totalPages,
                        Bets = bets.Value
                    };
                    PaginationCache.Store(response.Id, paginationData);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(10));
                        PaginationCache.Remove(response.Id);
                    });
                }
                else
                {
                    var result = await embedService.BuildErrorContainer(ctx.Client, $"unable to load bets at this time\r\n Bets: No Bets Found", ctx.Guild.Id, DiscordColor.DarkRed);
                    var errEmbed = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(result);
                    await ctx.EditResponseAsync(errEmbed);
                }
            }
        }
        #endregion

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
