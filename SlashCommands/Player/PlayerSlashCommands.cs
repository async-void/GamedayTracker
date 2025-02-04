using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Player
{

    public class PlayerSlashCommands
    {
        [DSharpPlus.Commands.Command("add")]
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
    }
}
