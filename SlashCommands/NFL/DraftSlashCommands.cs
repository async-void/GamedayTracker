using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.NFL
{
    [Command("Draft")]
    [Description("Draft group commands")]
    public class DraftSlashCommands
    {
        [Command("get")]
        [Description("Get supplied draft season")]
        public async Task GetDraftSeason(CommandContext ctx, [Parameter("season")] string season)
        {
            await ctx.DeferResponseAsync();

            await ctx.RespondAsync($"WIP: Get draft season {season}");
        }
    }
}
