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
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using Serilog;
using ILogger = GamedayTracker.Interfaces.ILogger;

namespace GamedayTracker.SlashCommands.NFL
{
    public class TeamScheduleSlashCommand(IGameData gameData, ITeamData teamData, IEspnClient espnClient)
    {
        private readonly IEspnClient _espnClient = espnClient;

        [Command("schedule")]
        [Description("Get Current Season Team Schedule")]
        public async Task GetTeamSchedule(SlashCommandContext ctx, [Parameter("team")] [Description("enter team abbreviation")] string teamName)
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
            var scheduleEspn = await _espnClient.GetTeamScheduleAsync(teamId.Value);
            var sb = new StringBuilder();
            var titleEmoji = NflEmojiService.GetEmoji(normalizedName?.ToAbbr() ?? "");
            if (scheduleEspn.Events.Count > 0)
            {
                foreach (var match in scheduleEspn.Events)
                {
                    var awayCompetitor = match.Competitions[0].Competitors[0];
                    var homeCompetitor = match.Competitions[0].Competitors[1];

                    var awayName = match.Competitions[0].Competitors[0].Team.Abbreviation;
                    var homeName = match.Competitions[0].Competitors[1].Team.Abbreviation;
                    var date = match.Date.Value.ToString("MMMM, dd yyyy hh:mm:ss tt");

                    var awayEmoji = NflEmojiService.GetEmoji(awayName);
                    var homeEmoji = NflEmojiService.GetEmoji(homeName);
                    var winEmoji = NflEmojiService.GetEmoji("Win");
                    var lossEmoji = NflEmojiService.GetEmoji("Loss");

                    var awayScore = match.Competitions[0].Competitors[0].LineScores?.Select(ls => ls.Value).Sum() ?? 0;
                    var homeScore = match.Competitions[0].Competitors[1].LineScores?.Select(ls => ls.Value).Sum() ?? 0;

                    string? result;
                    if (match.Competitions[0].Status.Type.Completed)
                    {
                        result = $"{awayEmoji} ``{awayScore}-{homeScore}`` {homeEmoji}";
                        sb.AppendLine($"{result} || FINAL");
                    }
                    else
                    {
                        sb.AppendLine($"{awayEmoji} at {homeEmoji} `{date, -30}`");
                    }    
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"{normalizedName} {titleEmoji}"),
                    new DiscordTextDisplayComponent($"-# {normalizedName} Schedule"),
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
                    new DiscordTextDisplayComponent($"no events found!"),
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
            var record = await _espnClient.GetTeam(teamAbbr);
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var emoji = NflEmojiService.GetEmoji(record.Abbreviation ?? "");
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"**{record.Name}** ({record.Abbreviation}) {emoji}"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent($"Summary: **{record.StandingSummary}**"),
                new DiscordTextDisplayComponent($"Home: **{record.Record.Items[1].Summary}** Road: **{record.Record.Items[2].Summary}**"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {timestamp}")
            ];
               
            var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.RespondAsync(msg);
        }

        #region
        [Command("teamRoster")]
        [Description("fetch a specific team's roster")]
        public async ValueTask GetRoster(SlashCommandContext ctx, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            var teamId = TeamExtensions.ToEspnTeamId(teamName.ToLower());
            var roster = await _espnClient.GetTeamRosterAsync(teamId.ToString());

            if (roster is not null)
            {
                if (roster.Count > 0)
                {
                    //TODO: add paging for the player roster data
                }
            }
        }
        #endregion
    }
}
