using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class RealTimeScoresJob(DiscordClient client, IEvaluator evaluator, ILogger<RealTimeScoresJob> logger, IEspnClient espnClient,
        CrosspostDispatcher crosspostDispatcher) : IJob
    {
        private readonly IEspnClient _espnClient = espnClient;
        private readonly DiscordClient _client = client;
        private readonly CrosspostDispatcher _crosspostDispatcher = crosspostDispatcher;

        private ulong? _liveScoreMsgId = null;

        public async Task Execute(IJobExecutionContext context)
        {
            
            var seasonType = evaluator.Evaluate(DateTimeOffset.Now);
            switch(seasonType)
            {
                case RealTimeScoresMode.Preseason:
                    logger.LogInformation("RealTimeScoresJob: Season is currently in Preseason mode. Interval - (24 hours)");
                    await UpdateTriggerInterval(context, TimeSpan.FromHours(24));
                    return;
                case RealTimeScoresMode.Offseason:
                    logger.LogInformation("RealTimeScoresJob: Season is currently in offseason mode. Interval - Offseason (7 days)");
                    await UpdateTriggerInterval(context, TimeSpan.FromDays(7));
                    return;
                case RealTimeScoresMode.PreGame:
                    logger.LogInformation("RealTimeScoresJob: Interval - PreGame (30 minutes)");
                    await UpdateTriggerInterval(context, TimeSpan.FromMinutes(30));
                    return;
                case RealTimeScoresMode.LiveGame:
                    logger.LogInformation("RealTimeScoresJob: Interval - LiveGame (1 minute)");
                    await UpdateTriggerInterval(context, TimeSpan.FromMinutes(1));
                    break;
                case RealTimeScoresMode.PostGame:
                    logger.LogInformation("RealTimeScoresJob: Interval - PostGame (1 hours)");
                    await UpdateTriggerInterval(context, TimeSpan.FromHours(1));
                    return;
            }
           
            try
            {
                var scoreboard = await _espnClient.GetScoreboardAsync("", "", "");
                var chnl = await _client.GetChannelAsync(1398021337498390539);
                var emoji = NflEmojiService.GetEmoji("default");
                var season = await _espnClient.GetSeasonAsync();
                var inProgress = scoreboard.Events.Any(e => e.Status.Type.Name == "STATUS_IN_PROGRESS");
                var completed = scoreboard.Events.Any(e => e.Status.Type.Name == "STATUS_FINAL");
                var scheduled = scoreboard.Events.Any(e => e.Status.Type.Name == "STATUS_SCHEDULED");
                var sb = new StringBuilder();

                if (inProgress)
                {
                    sb.AppendLine($"\r\n**In Progress**");
                    foreach (var game in scoreboard.Events.Where(e => e.Status.Type.Name == "STATUS_IN_PROGRESS"))
                    {
                        var homeTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "home");
                        var awayTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "away");
                        var awayLineScores = awayTeam?.LineScores != null
                                ? string.Join("-", awayTeam.LineScores.Select(ls => ls.Value))
                                : "";
                        var homeLineScores = homeTeam?.LineScores != null
                                ? string.Join("-", homeTeam.LineScores.Select(ls => ls.Value))
                                : "";
                        var awayScore = awayTeam?.LineScores.Select(ls => ls.Value).Sum() ?? 0;
                        var homeScore = homeTeam?.LineScores.Select(ls => ls.Value).Sum() ?? 0;
                        var homeEmoji = NflEmojiService.GetEmoji(homeTeam?.Team.Abbreviation ?? "default");
                        var awayEmoji = NflEmojiService.GetEmoji(awayTeam?.Team.Abbreviation ?? "default");
                        var playClock = $"qt:{game.Status.Period} | clock: {game.Status.DisplayClock}";
                        sb.AppendLine($"{awayEmoji} ``{awayScore}``-``{homeScore}`` {homeEmoji} \t {playClock.PadLeft(2)}");

                    }
                }
                if (completed)
                {
                    sb.AppendLine($"\r\n**Final**");
                    foreach (var game in scoreboard.Events.Where(e => e.Status.Type.Name == "STATUS_FINAL"))
                    {
                        var homeTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "home");
                        var awayTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "away");
                        var awayFinalScore = awayTeam?.LineScores.Select(ls => ls.Value).Sum();

                        var homeFinalScore = homeTeam.LineScores.Select(ls => ls.Value).Sum();

                        var homeEmoji = NflEmojiService.GetEmoji(homeTeam?.Team.Abbreviation ?? "default");
                        var awayEmoji = NflEmojiService.GetEmoji(awayTeam?.Team.Abbreviation ?? "default");

                        // Build labels
                        var awayLabel = $"{awayEmoji} ({awayFinalScore})";
                        var homeLabel = $"({homeFinalScore}) {homeEmoji}";

                        // Bold the winner
                        if (awayFinalScore > homeFinalScore)
                        {
                            awayLabel = $"**{awayLabel}**";
                        }
                        else if (homeFinalScore > awayFinalScore)
                        {
                            homeLabel = $"**{homeLabel}**";
                        }

                        sb.AppendLine($"{awayLabel}-{homeLabel}");

                    }
                }
                if (scheduled)
                {
                    sb.AppendLine($"\r\n**Scheduled**");
                    foreach (var game in scoreboard.Events.Where(e => e.Status.Type.Name == "STATUS_SCHEDULED"))
                    {
                        var homeTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "home");
                        var awayTeam = game.Competitions[0].Competitors.FirstOrDefault(c => c.HomeAway == "away");

                        var homeEmoji = NflEmojiService.GetEmoji(homeTeam?.Team.Abbreviation ?? "default");
                        var awayEmoji = NflEmojiService.GetEmoji(awayTeam?.Team.Abbreviation ?? "default");
                        sb.AppendLine($"{awayEmoji} @ {homeEmoji} - {game.Date.Value}");

                    }
                }

                var updatedTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent($"Realtime Scores"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"**{season.DisplayName}: Week {scoreboard.Week.Number}**"),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent("Gameday Tracker ©️ 2026"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donate", "Donate")) ,
                    new DiscordTextDisplayComponent($"last updated {updatedTimestamp}")
                ];
                var container = new DiscordContainerComponent(comps, false, DiscordColor.DarkBlue);
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                if (_liveScoreMsgId == null)
                {
                    var sentMsg = await chnl.SendMessageAsync(msg);
                    _liveScoreMsgId = sentMsg.Id;
                }
                else
                {
                    try
                    {
                        var existingMsg = await chnl.GetMessageAsync(_liveScoreMsgId.Value, true);
                        await existingMsg.ModifyAsync(msg);

                    }
                    catch (NotFoundException ex)
                    {
                        var sentMsg = await chnl.SendMessageAsync(msg);
                        _liveScoreMsgId = sentMsg.Id;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Fetching realtime scores....[failed] REASON: {reason}", ex.Message);
                return;
            }
            
        }

        private async Task UpdateTriggerInterval(IJobExecutionContext ctx, TimeSpan newInterval)
        {
            var scheduler = ctx.Scheduler;
            var triggerKey = ctx.Trigger.Key;

            var current = ctx.Trigger as ISimpleTrigger;

            if (current == null || current.RepeatInterval != newInterval)
            {
                var newTrigger = TriggerBuilder.Create()
                    .ForJob(ctx.JobDetail.Key)
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(newInterval)
                        .RepeatForever())
                    .Build();

                await scheduler.RescheduleJob(triggerKey, newTrigger);
            }
        }

    }
}
