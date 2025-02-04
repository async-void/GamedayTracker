using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;



namespace GamedayTracker.SlashCommands.Settings
{
    [Command("user-settings")]
    public class SettingsSlashCommands
    {
        [Command("favorite-team")]
        [Description("set's the user's favorite NFL team.")]
        public async Task SetFavoriteTeam(CommandContext ctx,
            [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();

            var message = new DiscordMessageBuilder()
                .WithContent("help slashcommand");

            await ctx.EditResponseAsync(message);
        }
    }
}
