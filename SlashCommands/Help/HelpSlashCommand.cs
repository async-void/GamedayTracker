using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace GamedayTracker.SlashCommands.Help
{
    public class HelpSlashCommand
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
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "userSettingsBtn", "User Settings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "newsBtn", "News")
            };
           

            var message = new DiscordMessageBuilder()
                .WithContent("a list of help topics")
                .AddComponents(buttons);
            
            
            await ctx.EditResponseAsync(message);
        }
    }
}
