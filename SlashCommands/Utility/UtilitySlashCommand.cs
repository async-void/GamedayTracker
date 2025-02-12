using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Help
{
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand
    {
        [Command("help")]
        [Description("a list of commands and a brief explaination")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var buttons = new DiscordComponent[]
            {
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "scoreboardBtn", "Scoreboard"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "standingsBtn", "Standings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "draftBtn", "Draft"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "userSettingsBtn", "User Settings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "newsBtn", "News")
            };

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Blurple)
                    .WithTitle("Help")
                    .WithFooter("Gameday Tracker")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    ).AddComponents(buttons);
            
            await ctx.EditResponseAsync(message);
        }

        [Command("ping")]
        [RequirePermissions(permissions: DiscordPermission.ManageGuild)]
        [Description("get the client latency [must have 'mod' or higher role]")]
        public async ValueTask Ping(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var guildId = ctx.Guild!.Id;
            var connectionLat = ctx.Client.GetConnectionLatency(guildId);
            //TODO: finish me.
            await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent($"{connectionLat.Nanoseconds.ToString()}ns"));
        }
    }
}
