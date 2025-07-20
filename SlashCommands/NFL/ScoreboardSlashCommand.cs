using DSharpPlus.Commands;
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
        public async Task GetScoreboard(CommandContext ctx, [SlashChoiceProvider<SeasonChoiceProvider>] int season,
            [SlashChoiceProvider<WeekChoiceProvider>] int week)
        {
            await ctx.DeferResponseAsync();
            var sBuilder = new StringBuilder();
            var scoreBoardResult = await gameService.GetScoreboard(season, week);
           
            var newWeek = week.ToString();
            if (scoreBoardResult.IsOk)
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
                        //Console.WriteLine($"{awayEmoji} {awayScore} -" +
                        //    $"**{homeScore}** {homeEmoji.PadRight(8)} - **FINAL**\r\n");
                    }
                       
                }
 
                newWeek = week switch
                {
                    19 => "Wildcard Playoffs",
                    20 => "Divisional Playoffs",
                    21 => "Conference Playoffs",
                    22 => "Super Bowl",
                    _ => $"Week {week.ToString()}"
                };

                var components = new DiscordComponent[]
                {
                    new DiscordTextDisplayComponent($"# {newWeek}"),
                    new DiscordTextDisplayComponent($"### Season {season}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent(sBuilder.ToString()),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow:MM-dd-yyy hh:mm:ss tt zzz}"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                };

                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            else
            {
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddTextDisplayComponent($"unable to fetch scoreboard for season: {season} {newWeek}");

                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            
        }
    }
}
