using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.SlashCommands.Settings
{
    public class SettingsSlashCommands
    {
        [Command("favorite-team")]
        [Description("set's the user's favorite NFL team.")]
        public async Task SetFavoriteTeam(CommandContext ctx,
            [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var userName = ctx.Member!.Username;

            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbMember = db.Members.Where(x => x.MemberName.Equals(userName))!.FirstOrDefault();

            if (dbMember is not null)
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription("WIP: member is in db, favorite-team command was run....")
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
        }
    }
}
