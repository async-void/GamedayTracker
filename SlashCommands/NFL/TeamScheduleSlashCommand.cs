using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Serilog;
using ILogger = GamedayTracker.Interfaces.ILogger;

namespace GamedayTracker.SlashCommands.NFL
{
    public class TeamScheduleSlashCommand(IGameData gameData, ITeamData teamData)
    {
        [Command("schedule")]
        [Description("Get Current Season Team Schedule")]
        public async Task GetTeamSchedule(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var normalizedName = NflTeamMatcher.MatchTeam(teamName);
            if (!teamData.IsValidTeamName(normalizedName!.ToLower()))
            {
                await ctx.EditResponseAsync(new DiscordMessageBuilder()
                        .WithContent($"Invalid team name: {teamName}. Please use a valid team name."))
                    .ConfigureAwait(false);
                return;
            }
            var sb = new StringBuilder();
            var teamSchedule = await gameData.GetTeamSchedule(normalizedName);
            var season = DateTime.UtcNow.Year.ToString();
            var titleEmoji = NflEmojiService.GetEmoji(normalizedName.ToAbbr());
            if (teamSchedule.IsOk)
            {
                foreach (var match in teamSchedule.Value)
                {
                    var awayName = match.Opponents!.AwayTeam.Name.ToAbbr();
                    var homeName = match.Opponents.HomeTeam.Name.ToAbbr();
                    var date = match.GameDate;
                    var awayEmoji = NflEmojiService.GetEmoji(awayName);
                    var homeEmoji = NflEmojiService.GetEmoji(homeName);
                    sb.AppendLine($"{awayEmoji} at {homeEmoji} `{date!}`");
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"{normalizedName} {titleEmoji}"),
                    new DiscordTextDisplayComponent($"-# {season} Schedule"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];

                var container = new DiscordContainerComponent(components);

                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(message);
            }
            else
            {
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"**ERROR**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"{teamSchedule.Error.ErrorMessage} for season **{season}**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>")
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.IndianRed);
                var errorEmbed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.EditResponseAsync(errorEmbed);
            }
        }

        [Command("team-record")]
        [Description("get the supplied team's record ex: PIT")]
        public async Task GetTeamRecord(SlashCommandContext ctx, string teamAbbr)
        {
            await ctx.DeferResponseAsync();
            var record = await teamData.GetTeamRecordAsync(teamAbbr);
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"**Record for: {record.Item2.DisplayName}** ({record.Item2.Abbreviation})"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"Summary: **{record.Item1}**"),
                new DiscordTextDisplayComponent($"Home: {record.Item2.Record.Items[1].Summary} Road: {record.Item2.Record.Items[2].Summary}"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}")
            ];
               
            var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.RespondAsync(msg);
        }
    }
}
