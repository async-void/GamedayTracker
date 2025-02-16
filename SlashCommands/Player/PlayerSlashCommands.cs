using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
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
        public async Task AddPlayerPicks(CommandContext ctx,
            [Parameter("member")] DiscordUser user, [Parameter("picks")] string[] picks)
        {
            await ctx.DeferResponseAsync();
            //await ctx.EditResponseAsync("fetching this weeks matchups...this may take a moment...");
            await using var db = new AppDbContextFactory().CreateDbContext();
            var matchups = db.Matchups.Where(x => x.Season == 2024 && x.Week == 1)
                .Include(x => x.Opponents)
                .Include(x => x.Opponents.AwayTeam)
                .Include(x => x.Opponents.HomeTeam)
                .ToList();
            var buttons = new DiscordComponent[]
            {
                new DiscordButtonComponent(DiscordButtonStyle.Primary, $"m{matchups[0].Opponents.AwayTeam.Name}", $"{matchups[0].Opponents.AwayTeam.Name}"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, $"m{matchups[0].Opponents.HomeTeam.Name}", $"{matchups[0].Opponents.HomeTeam.Name}")
            };
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Add Player Picks!")
                    .WithDescription(
                        "add player picks is not yet implemented. the bot devs are hard at work with the next update.")
                    .WithTimestamp(DateTime.UtcNow)
                    ).AddComponents(buttons);

            await ctx.EditResponseAsync(message);
        }
        #endregion
    }
}
