using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Factories;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands
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
            [Parameter("member")] DiscordUser user, [Parameter("picks")] string[] picks)
        {
            //await ctx.Interaction.DeferAsync();
           
            await using var db = new AppDbContextFactory().CreateDbContext();
            var matchups = db.Matchups.Where(x => x.Season == 2024 && x.Week == 1)
                .Include(x => x.Opponents)
                .Include(x => x.Opponents.AwayTeam)
                .Include(x => x.Opponents.HomeTeam)
                .ToList();

            var pages = new List<Page>();

            foreach (var matchup in matchups)
            {
                var embedM = new DiscordEmbedBuilder()
                    .WithTitle($"Week {matchup.Week} Matchup")
                    .WithDescription($"{matchup.Opponents.AwayTeam.Name} vs {matchup.Opponents.HomeTeam.Name}")
                    .WithTimestamp(DateTime.UtcNow);
                var buttons = new DiscordComponent[2]
                {
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, $"m{matchup.Opponents.AwayTeam.Name}", $"{matchup.Opponents.AwayTeam.Name}"),
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, $"m{matchup.Opponents.HomeTeam.Name}", $"{matchup.Opponents.HomeTeam.Name}")
                };
                ;
                pages.Add(new Page($"matchup", embedM));
            }

            await ctx.Interaction.SendPaginatedResponseAsync(false, user, pages);
        }
        #endregion
    }
}
