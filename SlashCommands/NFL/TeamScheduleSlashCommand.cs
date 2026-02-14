using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
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
        public async Task GetTeamSchedule(SlashCommandContext ctx, [Parameter("team")] string teamName, [SlashChoiceProvider<SeasonTypeChoiceProvider>] int seasonType, [SlashChoiceProvider<SeasonChoiceProvider>]int season)
        {
            
            await ctx.DeferResponseAsync();
           
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var normalizedName = NflTeamMatcher.MatchTeam(teamName);
            var teamId = teamData.GetIdFromTeamName(teamName);
           
            if (!teamId.IsOk)
            {
                await ctx.EditResponseAsync(new DiscordMessageBuilder()
                        .WithContent($"Invalid team name: {teamName}. Please use a valid team name."))
                    .ConfigureAwait(false);
                return;
            }
            var scheduleEspn = await gameData.GetEspnTeamScheduleAsync(teamId.Value, seasonType, season);
            var sb = new StringBuilder();
            var titleEmoji = NflEmojiService.GetEmoji(normalizedName.ToAbbr());
            if (scheduleEspn.Events.Count > 0)
            {
                foreach (var match in scheduleEspn.Events)
                {
                    var awayCompetitor = match.Competitions[0].Competitors[0];
                    var homeCompetitor = match.Competitions[0].Competitors[1];

                    var awayName = match.Competitions[0].Competitors[0].Team.Abbreviation;
                    var homeName = match.Competitions[0].Competitors[1].Team.Abbreviation;
                    var date = match.Date.ToString("MMMM, dd yyyy hh:mm:ss tt");

                    var awayEmoji = NflEmojiService.GetEmoji(awayName);
                    var homeEmoji = NflEmojiService.GetEmoji(homeName);
                    var winEmoji = NflEmojiService.GetEmoji("Win");
                    var lossEmoji = NflEmojiService.GetEmoji("Loss");

                    string? result;
                    if (match.Competitions[0].Status.Type.Completed)
                    {
                        if (awayCompetitor.Winner)
                        {
                            result = $"{awayEmoji} ✔️ at {homeEmoji} ❗";
                        }
                        else if (homeCompetitor.Winner)
                        {
                            result = $"{awayEmoji} ❗ at {homeEmoji} ✔️";
                        }
                        else
                        {
                            result = $"{awayEmoji} at {homeEmoji} (TIE)";
                        }

                        sb.AppendLine($"{result} `{date,-30}`");
                    }
                    else
                        sb.AppendLine($"{awayEmoji} at {homeEmoji} `{date,-30}`");

                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"{normalizedName} {titleEmoji}"),
                    new DiscordTextDisplayComponent($"-# {season} Schedule"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}"),
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
                    new DiscordTextDisplayComponent($"no events found for season **{season}**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}")
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
