using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands
    {
        private readonly ICommandHelper _slashCmdHelper;

        public BetSlashCommands(ICommandHelper slashCmdHelper)
        {
            _slashCmdHelper = slashCmdHelper;
        }

        [Command("bet")]
        [Description("make a bet on a matchup")]
        public async Task Bet(CommandContext ctx, [Description("The team you are betting on to win the game")] string team, [Description("The amount you are betting")] int amount)
        {
            await ctx.RespondAsync($"You bet {amount} on {team}");
        }

        [Command("leaderboard")]
        [Description("get the betting leaderboard")]
        public async Task Leaderboard(CommandContext ctx, [SlashChoiceProvider<LeaderboardChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            await using var db = new BotDbContextFactory().CreateDbContext();

            switch (choice)
            {
                case 0:
                    var leaderboard = _slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id.ToString(), choice);
                    //TODO: build embed
                    break;
                case 1:
                    leaderboard = _slashCmdHelper.BuildLeaderboard(ctx.Guild!.Id.ToString(), choice);
                    //TODO: build embed
                    break;
                default:
                    await ctx.RespondAsync("Unknown choice - command will be ignored");
                    break;
            }
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Leaderboard")
                    .WithDescription("wip - trying to build a embed with select options")
                    .WithColor(DiscordColor.Cyan));


            await ctx.RespondAsync(message);
        }
    }
}
