using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Help
{
    public class HelpSlashCommand
    {
        [Command("help")]
        [Description("a list of commands and a brief explaination")]
        public async ValueTask Help(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            await ctx.EditResponseAsync("Building help context...");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Help Context")
                    .WithDescription("a list of commands and a brief explaination"));

            await ctx.EditResponseAsync(message);
        }
    }
}
