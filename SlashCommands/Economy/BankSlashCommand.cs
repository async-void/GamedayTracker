using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Economy
{
    public class BankSlashCommand
    {
        [DSharpPlus.Commands.Command("balance")]
        [Description("Get User Bank Balance")]
        public async Task GetUserBalance(CommandContext ctx,
            [Option("member", "select a member to view their bank balance")] DiscordUser user)
        {
            await ctx.DeferResponseAsync();

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"User Balance for {user.GlobalName}")
                    .WithDescription("WIP: this gets member's bank balance")
                    .WithTimestamp(DateTime.UtcNow));

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }
    }
}
