using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ScoreboardSlashCommand
    {
        private readonly GameDataService _gameService = new();

        [Command("scoreboard")]
        [Description("get the scores for a specified week")]
        public async Task GetScoreboard(CommandContext ctx, [Parameter("season")] string season,
            [Parameter("week")] string week)
        {

            var parseSeasonResult = int.TryParse(season, out var seasonParsed);
            var parsedWeekResult = int.TryParse(week, out var weekParsed);
            var sBuilder = new StringBuilder();

            var scoreBoardResult = _gameService.GetScoreboard(seasonParsed, weekParsed);
            
            await ctx.DeferResponseAsync();

            if (scoreBoardResult.IsOk)
            {
                var _season = scoreBoardResult.Value[0].Season;
                var _week = scoreBoardResult.Value[0].Week;

                sBuilder.Append($"**Season {_season}: Week {_week}**\r\n\r\n");
                for (int i = 0; i < scoreBoardResult.Value.Count; i++)
                {
                    var awayName = scoreBoardResult.Value[i].Opponents.AwayTeam.Name;
                    var awayScore = scoreBoardResult.Value[i].Opponents.AwayTeam.Score;
                    var awayRecord = scoreBoardResult.Value[i].Opponents.AwayTeam.Record;
                    var awayEmoji = scoreBoardResult.Value[i].Opponents.AwayTeam.Emoji;

                    var homeName = scoreBoardResult.Value[i].Opponents.HomeTeam.Name;
                    var homeScore = scoreBoardResult.Value[i].Opponents.HomeTeam.Score;
                    var homeRecord = scoreBoardResult.Value[i].Opponents.HomeTeam.Record;
                    var homeEmoji = scoreBoardResult.Value[i].Opponents.HomeTeam.Emoji;

                    sBuilder.Append(
                        $"{awayEmoji} **{awayScore}** -" +
                        $"**{homeScore}** {homeEmoji}\t\t\\|| FINAL\r\n" );
                }
                
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Gameday Tracker Scoreboard")
                        .WithDescription(sBuilder.ToString())
                        .WithThumbnail("https://i.imgur.com/jj94UiI.png", 64, 64)
                        .WithTimestamp(DateTimeOffset.UtcNow));

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
