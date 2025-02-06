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

            var member = ctx.Member;
            await using var db = new BotDbContextFactory().CreateDbContext();

            // check if user is in the db. consider making a util function to do the following.
            var dbUser = db.Members.Where(x => x.MemberName.Equals(member!.Username))!.FirstOrDefault();
            //user is in db, run daily command.
            if (dbUser is not null)
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription("WIP: member is in db, daily command was run....")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription("WIP: member is not in db\r\nwould you like to add the member now?")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            //user is not in db, add user to db then run daily.

            
        }
    }
}
