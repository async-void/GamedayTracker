﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands(ICommandHelper slashCmdHelper, IGameData gameData)
    {
        [Command("bet")]
        [Description("place bet on a matchup")]
        public async Task Bet(SlashCommandContext ctx, [Parameter("amount")] int amount, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var games = gameData.GetCurrentScoreboard();

            if (games.IsOk)
                await ctx.RespondAsync($"You bet {amount} on {teamName}");
            else
            {
                await ctx.RespondAsync(games.Error.ErrorMessage!);
            }
        }

        #region LEADERBOARD
        [Command("leaderboard")]
        [Description("get the betting leaderboard")]
        public async Task Leaderboard(CommandContext ctx, [SlashChoiceProvider<LeaderboardChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            await using var db = new BotDbContextFactory().CreateDbContext();

            var leaderboard = slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id.ToString(), choice);

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
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLocalTime()}")
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
