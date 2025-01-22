using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands
{
    public class ScoreboardSlashCommand : ApplicationCommandModule
    {
        private readonly GameDataService _gameService = new();

        [SlashCommand("scoreboard", "Get scoreboards for a season and a week")]
        public async Task GetScoreboard(InteractionContext ctx, [Option("season", "enter the year")] string season,
            [Option("week", "enter the week")] string week)
        {

            var parseSeasonResult = int.TryParse(season, out var seasonParsed);
            var parsedWeekResult = int.TryParse(week, out var weekParsed);
            var sBuilder = new StringBuilder();

            var scoreBoardResult = _gameService.GetScoreboard(seasonParsed, weekParsed);
            await ctx.DeferAsync();

            if (scoreBoardResult.IsOk)
            {
                for (int i = 0; i < scoreBoardResult.Value.Count; i++)
                {
                    sBuilder.Append(
                        $"{scoreBoardResult.Value[i].Opponents.AwayTeam.Name} {scoreBoardResult.Value[i].Opponents.AwayTeam.Record} at " +
                        $"{scoreBoardResult.Value[i].Opponents.HomeTeam.Name} {scoreBoardResult.Value[i].Opponents.HomeTeam.Record}\r\n" );
                }
                
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Gameday Tracker")
                        .WithDescription(sBuilder.ToString()));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Gameday Tracker")
                        .WithDescription($"{scoreBoardResult.Error.ErrorMessage}")
                        .WithTimestamp(DateTime.UtcNow));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            
        }
    }
}
