using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.NFL
{
    public class TeamScheduleSlashCommand(IGameData gameData, ITeamData teamData, ILogger logger)
    {
        [Command("schedule")]
        [Description("Get Team Schedule")]
        public async Task GetTeamSchedule(SlashCommandContext ctx, [Parameter("team")] string teamName, [Parameter("season")] int season)
        {
            
            await ctx.DeferResponseAsync();
            if (!teamData.IsValidTeamName(teamName.ToLower()))
            {
                await ctx.EditResponseAsync(new DiscordMessageBuilder()
                        .WithContent($"Invalid team name: {teamName}. Please use a valid team name."))
                    .ConfigureAwait(false);
                return;
            }
            var sb = new StringBuilder();
            var teamSchedule = await gameData.GetTeamSchedule(teamName, season);

            if (teamSchedule.IsOk)
            {
                foreach (var team in teamSchedule.Value)
                {
                    var vsName = team.Split("-")[0].Trim().ToAbbr();
                    var date = team.Split("-")[1].Trim();
                    var emoji = NflEmojiService.GetEmoji(vsName);
                    sb.AppendLine($"{date}: {vsName} {emoji}\r");
                }
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"{season} Schedule for {teamName}")
                        .WithDescription(sb.ToString())
                        .WithColor(DiscordColor.Blurple)
                        .WithTimestamp(DateTimeOffset.UtcNow));
                await ctx.EditResponseAsync(message);
                logger.Log(LogTarget.Console, LogType.Information, DateTimeOffset.UtcNow, $"[Get Team Schedule] method ran in debug mode. |server [{ctx.Guild!.Name}]| user: [{ctx.Member!.Username}]");
            }
            else
            {
                var errorEmbed = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(ctx.Client.CurrentUser.Username)
                        .WithDescription($"Error\r\n{teamSchedule.Error.ErrorMessage}")
                        .WithColor(DiscordColor.Blurple)
                        .WithTimestamp(DateTimeOffset.UtcNow));
                await ctx.EditResponseAsync(errorEmbed);
            }
        }
    }
}
