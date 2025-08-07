using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ScoreboardSlashCommand(IGameData gameService)
    {

        [Command("scoreboard")]
        [Description("get the scores for a specified week")]
        public async Task GetScoreboard(SlashCommandContext ctx, [SlashChoiceProvider<SeasonChoiceProvider>] int season,
            [SlashChoiceProvider<WeekChoiceProvider>] int week)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var sBuilder = new StringBuilder();
            var scoreBoardResult = await gameService.GetScoreboard(season, week);
           
            var newWeek = week.ToString();
            if (scoreBoardResult.IsOk && scoreBoardResult.Value.Count > 0)
            {
                for (int i = 0; i < scoreBoardResult.Value.Count; i++)
                {
                    var awayName = scoreBoardResult.Value[i].Opponents!.AwayTeam.Name;
                    var awayScore = scoreBoardResult.Value[i].Opponents!.AwayTeam.Score;
                    var awayRecord = scoreBoardResult.Value[i].Opponents!.AwayTeam.Record;
                    var awayEmoji = scoreBoardResult.Value[i].Opponents!.AwayTeam.Emoji;

                    var homeName = scoreBoardResult.Value[i].Opponents!.HomeTeam.Name;
                    var homeScore = scoreBoardResult.Value[i].Opponents!.HomeTeam.Score;
                    var homeRecord = scoreBoardResult.Value[i].Opponents!.HomeTeam.Record;
                    var homeEmoji = scoreBoardResult.Value[i].Opponents!.HomeTeam.Emoji;
                   
                    if (awayScore > homeScore)
                    {
                        sBuilder.Append(
                            $"{awayEmoji} **{awayScore}** -" +
                            $"{homeScore} {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                        //Console.WriteLine($"{awayEmoji} {awayScore} -" +
                        //   $"**{homeScore}** {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                    }
                    else
                    {
                        sBuilder.Append(
                        $"{awayEmoji} {awayScore} -" +
                        $"**{homeScore}** {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                    } 
                }
 
                newWeek = week switch
                {
                    19 => "Wildcard Playoffs",
                    20 => "Divisional Playoffs",
                    21 => "Conference Playoffs",
                    22 => "Super Bowl",
                    _ => $"Week {week}"
                };

                var components = new DiscordComponent[]
                {
                    new DiscordTextDisplayComponent($"# {newWeek}"),
                    new DiscordTextDisplayComponent($"### Season {season}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent(sBuilder.ToString()),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                };

                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(message);
            }
            else
            {
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddTextDisplayComponent($"No data found for Season: {season} Week {week}");

                await ctx.RespondAsync(message);
            }
            
        }
    }
}
