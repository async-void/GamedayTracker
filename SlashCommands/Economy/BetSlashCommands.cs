﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands
    {

        [Command("bet")]
        [Description("make a bet on a matchup")]
        public async Task Bet(CommandContext ctx, [Description("The team you are betting on to win the game")] string team, [Description("The amount you are betting")] int amount)
        {
            await ctx.RespondAsync($"You bet {amount} on {team}");
        }

        [Command("leaderboard")]
        [Description("get the betting leaderboard")]
        public async Task Leaderboard(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var options = new DiscordSelectComponentOption[]
            {
                new("Server", "ldbServer"),
                new("Global", "ldbGlobal"),
            };

            var menu = new DiscordComponent[]
            {
                new DiscordSelectComponent("ldbOptions", "Options", options)
            };

            var message = new DiscordMessageBuilder()
                .AddComponents(menu)
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Leaderboard")
                    .WithDescription("wip - trying to build a embed with select options")
                    .WithColor(DiscordColor.Cyan));


            await ctx.RespondAsync(message);
        }
    }
}
