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

                var components = new DiscordComponent[]
                {
                    new DiscordTextDisplayComponent($"**Season {season}: Week {week}**\r\n\r\n"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent(sBuilder.ToString()),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}")
                };

                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                logger.Log(LogTarget.Console, LogType.Debug, DateTimeOffset.UtcNow, $"Scoreboard command used | {ctx.Guild!.Name} | user: {ctx.User.Username}");
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            else
            {
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddTextDisplayComponent($"unable to fetch scoreboard for season: {season} week: {week}");
                
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            
        }
    }
}
