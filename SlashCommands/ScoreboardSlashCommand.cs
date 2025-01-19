using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands
{
   
    [SlashCommandGroup("Scoreboard", "Scoreboard Commands")]
    public class ScoreboardSlashCommand: ApplicationCommandModule
    {
        private readonly GameDataService _gameService = new();

        [SlashCommand("scoreboard", "get scoreboard for a season and a week")]
        public async Task GetScoreboard(InteractionContext ctx, [Option("season", "enter the year")] string season,
            [Option("week", "enter the week")] string week)
        {
            var parseSeasonResult = int.TryParse(season, out var seasonParsed);
            var parsedWeekResult = int.TryParse(week, out var weekParsed);

            var scoreBoardResult = _gameService.GetScoreboard(seasonParsed, weekParsed);

            await ctx.DeferAsync();
            var embed = new DiscordEmbedBuilder().WithTitle("Testing Response");

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
