using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;

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
            var user = ctx.Member!.Username;

            await using var db = new BotDbContextFactory().CreateDbContext();
            var dbMember = db.Members.Where(x => x.Member.Username.Equals(user))!.FirstOrDefault();

            var message = new DiscordMessageBuilder()
                .WithContent("help slashcommand");

            await ctx.EditResponseAsync(message);
        }
    }
}
