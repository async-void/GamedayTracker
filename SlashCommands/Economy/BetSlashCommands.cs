using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using GamedayTracker.Utility.Multipliers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static GamedayTracker.Utility.Multipliers.BetMultiplier;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands(ICommandHelper slashCmdHelper, IJsonDataService jsonService, IGameData gameDataService, IBetting bettingService)
    {
        private readonly IJsonDataService _jsonService = jsonService;
        private readonly IGameData _gameDataService = gameDataService;
        private readonly IBetting _bettingService = bettingService;
        private readonly ICommandHelper _slashCmdHelper = slashCmdHelper;

        [Command("bet")]
        [Description("place bet on a matchup")]
        public async Task Bet(SlashCommandContext ctx, [Parameter("amount")] int amount, [Parameter("team")] string teamName)
        {
            //TODO: finish betting command
            await ctx.DeferResponseAsync();

            var scoreboard = await _gameDataService.GetNFLScoresAsync();

            var scheduled = scoreboard.Events
                .Where(s => s.Status.Type.Description.Equals("Scheduled"))
                .ToList();
            if (!scheduled.Any())
            {
                await ctx.RespondAsync("No scheduled games found, unable to place bet at this time.");
                return;
            }

            //here we build the list of available games to bet on from the scheduled games.
            IEnumerable<DiscordSelectComponentOption> gameOptions = scheduled.Select(s =>
            {
                var awayTeam = s.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway.Equals("away"))?.Team.DisplayName ?? "Unknown";
                var homeTeam = s.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway.Equals("home"))?.Team.DisplayName ?? "Unknown";
                var optionLabel = $"{awayTeam} at {homeTeam}";
                var optionValue = $"{s.Competitions[0].Id}";
                return new DiscordSelectComponentOption(optionLabel, optionValue);
            });

            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent("### Place Your Bet"),
                new DiscordSeparatorComponent(true),
                new DiscordActionRowComponent([new DiscordSelectComponent("test", "test", gameOptions)]),
            ];

            var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);
            var embed = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.EditResponseAsync(embed);
            //var member = await _jsonService.GetMemberFromJsonAsync(ctx.User.Id.ToString(), ctx.Guild!.Id.ToString());
            //if (member.IsOk)
            //{
            //    var matchups = await _gameDataService.GetScoreboard(2024, 1);
            //    if (matchups.IsOk)
            //    {
            //        var matchup = matchups.Value.FirstOrDefault(m => m.Opponents!.AwayTeam.Name.Equals(teamName) || m.Opponents.HomeTeam.Name.Equals(teamName));
            //        if (matchup is { } m)
            //        {
            //            var multiplier = new BetMultiplier();
            //            var bonus =multiplier.GetMultiplier(BetType.Bonus);

            //            var bet = new Bet()
            //            {
            //                BetId = Guid.NewGuid(),
            //                MemberId = member.Value.MemberId,
            //                Matchup = matchup,
            //                BetAmount = amount,
            //                Multiplier = bonus,
            //                TeamPickedToWinName = teamName,
            //                GuildId = ctx.Guild.Id.ToString()
            //            };
            //            var isValidBet = await _bettingService.CanPlaceBet(m, bet, member.Value);
            //            if (isValidBet.IsOk)
            //            {
            //                //TODO: save the bet to the members json file.
            //                member.Value.Bets = [bet];
            //                var updatedMember = await _jsonService.UpdateMemberDataAsync(member.Value);

            //                if (!updatedMember.IsOk)
            //                {
            //                    await ctx.RespondAsync("Unable to update member data, bet not saved!");
            //                    return;
            //                }
                            
            //                //if we get this far ...we win.
            //                DiscordComponent[] cmps =
            //                [
            //                    new DiscordTextDisplayComponent($"### Member [{member.Value.MemberName}] Bet Info"),
            //                    new DiscordSeparatorComponent(true),
            //                    new DiscordTextDisplayComponent($"Amount: {bet.BetAmount}"),
            //                    new DiscordTextDisplayComponent($"Multiplier: {bet.Multiplier}"),
            //                    new DiscordTextDisplayComponent($"Team Name: {bet.TeamPickedToWinName}"),
            //                ];
            //                await ctx.RespondAsync($"-# You bet {amount} on {teamName}\r\n-# this will eventually be turned into an embed");
            //                return;
            //            }
            //            else
            //            {
            //                await ctx.RespondAsync($"-# this will eventually be an embed.\r\n{isValidBet.Error.ErrorCode} : {isValidBet.Error.ErrorMessage}");
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            await ctx.RespondAsync($"-# this will eventually be an embed.\r\nmatchup is either invalid or un reachable, unable to place bet at this time\r\n");
            //            return;
            //        }
            //    }
            //    await ctx.RespondAsync($"-# this will eventually be an embed.\r\nmatchups is either invalid or un reachable, unable to place bet at this time\r\n" +
            //        $"{matchups.Error.ErrorCode}: {matchups.Error.ErrorMessage}");
            //}
            //else
            //{
            //    //member isn't in the json file, create a new member and credit the balance.
            //    var user = ctx.Member?.Username ?? "not found";
            //    await ctx.RespondAsync($"Member isn't in the json file, I am creating an account and giving [{user}] 100 credits");
            //    return;
            //}
            //await ctx.RespondAsync($"Unable to place bet!");
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
                    leaderboard = await _slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id.ToString(), choice);
                    break;
                case 1:
                    leaderboard = await _slashCmdHelper.BuildLeaderboard("", choice);
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

            var embedDesc = _slashCmdHelper.BuildLeaderboardDescription(leaderboard.Value).Value;
            
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
