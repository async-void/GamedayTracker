using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.Economy
{
    [Command("betting")]
    [Description("betting slash commands")]
    public class BetSlashCommands
    {
        [Command("bet")]
        [Description("make a bet on a matchup")]
        public async Task Bet(CommandContext ctx, [Description("The team you are betting on to win the game")] string team, [Description("The amount you are betting")] int amount)
        {
            await ctx.RespondAsync($"You bet {amount} on {team}");
        }
    }
}
