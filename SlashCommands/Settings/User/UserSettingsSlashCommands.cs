using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Factories;

namespace GamedayTracker.SlashCommands.Settings.User
{
    [Command("user-settings")]
    [Description("set user settings")]
    public class UserSettingsSlashCommands
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
                dbMember.FavoriteTeam = teamName;
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"User Setting - Favorite Team Command ")
                        .WithDescription("WIP: member is in db, favorite-team command was run....")
                        .WithTimestamp(DateTime.UtcNow));

                db.Members.Update(dbMember);
                await db.SaveChangesAsync();
                await ctx.EditResponseAsync(message);
                await ctx.User.SendMessageAsync($"favorite team {teamName} has been set!");
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Daily Command")
                        .WithDescription("WIP: member is not in db\r\nwould you like to add the member now?")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(message);
            }
        }
    }
}
