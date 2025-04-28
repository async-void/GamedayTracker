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
                    sb.AppendLine($"{date}: {vsName} {emoji}");
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"Schedule {season} for {teamName}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent(sb.ToString()),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}:{DateTime.Now.ToShortTimeString()}")
                ];

                var container = new DiscordContainerComponent(components);

                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(message);
                logger.Log(LogTarget.Console, LogType.Information, DateTimeOffset.UtcNow, $"[Get Team Schedule] method ran in debug mode. |server [{ctx.Guild!.Name}]| user: [{ctx.Member!.Username}]");
            }
            else
            {
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"**ERROR**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"{teamSchedule.Error.ErrorMessage} for season **{season}**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}:{DateTime.Now.ToShortTimeString()}")
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.IndianRed);
                var errorEmbed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.EditResponseAsync(errorEmbed);
            }
        }
    }
}
