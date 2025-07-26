using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Factories;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands(ICommandHelper slashCmdHelper, IJsonDataService jsonService)
    {
        private readonly IJsonDataService _jsonService = jsonService;

        [Command("bet")]
        [Description("place bet on a matchup")]
        public async Task Bet(SlashCommandContext ctx, [Parameter("amount")] int amount, [Parameter("team")] string teamName)
        {
            //TODO: finish betting command
            await ctx.DeferResponseAsync();
            var member = await _jsonService.GetMemberFromJsonAsync(ctx.User.Id.ToString(), ctx.Guild!.Id.ToString());
            await ctx.RespondAsync($"-# You bet {amount} on {teamName}");
        }

        #region LEADERBOARD
        [Command("leaderboard")]
        [Description("get the betting leaderboard")]
        public async Task Leaderboard(SlashCommandContext ctx, [SlashChoiceProvider<LeaderboardChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            var leaderboard = new Result<List<GuildMember>, SystemError<SlashCommandHelper>>();

            switch (choice)
            {
                case 0:
                    leaderboard = await slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id.ToString(), choice);
                    break;
                case 1:
                    leaderboard = await slashCmdHelper.BuildLeaderboard("", choice);
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
                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {DateTime.UtcNow.ToLocalTime()}")
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
                new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLocalTime()}")
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
