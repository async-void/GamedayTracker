﻿using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands(IGameData gameData)
    {
        #region ADD PLAYER TO POOL

        [Command("add")]
        [Description("add player to the pool")]
        public async Task AddPlayer(CommandContext ctx,
            [Parameter("member")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Not Implemented Yet!")
                    .WithDescription(
                        "add player is not yet implemented. the bot devs are hard at work with the next update.")
                    .WithTimestamp(DateTime.UtcNow));
            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }

        #endregion

        #region ADD PLAYER PICKS

        [Command("picks")]
        [Description("add player picks")]
        public async Task AddPlayerPicks(SlashCommandContext ctx,
            [Parameter("member")] DiscordUser user, [Parameter("picks")] string picks)
        {
            await ctx.Interaction.DeferAsync();
            var teamSchedule = gameData.GetTeamSchedule("Buffalo");
            var playerPicks = picks.Split(" ").ToList();

            await using var db = new AppDbContextFactory().CreateDbContext();
            var matchups = db.Matchups.Where(x => x.Season == 2024 && x.Week == 1)
                .Include(x => x.Opponents)
                .Include(x => x.Opponents.AwayTeam)
                .Include(x => x.Opponents.HomeTeam)
                .ToList();

            var curWeek = gameData.GetCurWeek();
           //var matchupCount = gameData.GetMatchupCount();

            var isValid = playerPicks.Count > 1;

            if (!isValid)
            {
                var errMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Error")
                        .WithDescription($"picks must match game count ``[{matchups.Count}]``\r\nplease choose a team from every game")
                        .WithTimestamp(DateTime.UtcNow));
                await ctx.EditResponseAsync(errMessage);
                return;
            }
            
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Week {matchups[0].Week} Scoreboard")
                    .WithDescription($"player {user.Username} picks have been added")
                    .WithTimestamp(DateTime.UtcNow));
            await ctx.EditResponseAsync(message);
        }

        #endregion
    }
}
