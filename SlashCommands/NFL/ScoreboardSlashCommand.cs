using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ScoreboardSlashCommand(ILogger logger)
    {
        private readonly GameDataService _gameService = new();

        [Command("scoreboard")]
        [Description("get the scores for a specified week")]
        public async Task GetScoreboard(CommandContext ctx, [SlashChoiceProvider<SeasonChoiceProvider>] int season,
            [SlashChoiceProvider<WeekChoiceProvider>] int week)
        {

            var sBuilder = new StringBuilder();
            var scoreBoardResult = _gameService.GetScoreboard(season, week);
            
            await ctx.DeferResponseAsync();

            if (scoreBoardResult.IsOk)
            {
                sBuilder.Append($"**Season {season}: Week {week}**\r\n\r\n");
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

                    if (awayScore > homeScore)
                    {
                        sBuilder.Append(
                            $"{awayEmoji} **{awayScore}** -" +
                            $"{homeScore} {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                    }
                    else
                    {
                        sBuilder.Append(
                            $"{awayEmoji} {awayScore} -" +
                            $"**{homeScore}** {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                    }
                       
                }
                
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Gameday Tracker Scoreboard")
                        .WithDescription(sBuilder.ToString())
                        .WithThumbnail("https://i.imgur.com/jj94UiI.png", 64, 64)
                        .WithFooter("Gameday Tracker ")
                        .WithTimestamp(DateTimeOffset.UtcNow));
                logger.Log(LogTarget.Console, LogType.Debug, DateTimeOffset.UtcNow, $"Scoreboard command used | {ctx.Guild!.Name} | user: {ctx.User.Username}");
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
