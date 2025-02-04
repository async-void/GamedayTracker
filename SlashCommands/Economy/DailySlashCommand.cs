using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;

namespace GamedayTracker.SlashCommands.Economy
{
    public class DailySlashCommand
    {
        [Command("daily")]
        [Description("runs the daily command")]
        public async ValueTask Daily(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var user = ctx.Member;
            await using var db = new AppDbContextFactory().CreateDbContext();

            // check if user is in the db. consider making a util function to do the following.

            //user is in db, run daily command.

            //user is not in db, add user to db then run daily.

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Daily Command")
                    .WithDescription("WIP: this runs the daily command")
                    .WithTimestamp(DateTime.UtcNow));

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }
    }
}
