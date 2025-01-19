using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands
{
   
    [SlashCommandGroup("Scoreboard", "Scoreboard Commands")]
    public class ScoreboardSlashCommand : ApplicationCommandModule
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
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Gameday Tracker")
                    .WithThumbnail(scoreBoardResult.Value[0].Opponents.AwayTeam.LogoPath, 10, 10)
                    .AddField("Season", season, true)
                    .AddField("Week", week, true)
                    .AddField("Record", scoreBoardResult.Value[0].Opponents.AwayTeam.Record, true));

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }
    }
}
