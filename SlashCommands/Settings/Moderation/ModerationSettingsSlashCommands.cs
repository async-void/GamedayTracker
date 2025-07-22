using System.Text;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Quartz;
using Quartz.Impl.Matchers;
using CommandAttribute = DSharpPlus.Commands.CommandAttribute;
using RequirePermissionsAttribute = DSharpPlus.Commands.ContextChecks.RequirePermissionsAttribute;

namespace GamedayTracker.SlashCommands.Settings.Moderation
{
    [Command("moderation")]
    [Description("Moderation Slash Commands")]
    [RequirePermissions([DiscordPermission.Administrator, DiscordPermission.ManageGuild])]
    public class ModerationSettingsSlashCommands(IJsonDataService jsonService, ISchedulerFactory schedulerFactory)
    {
        private readonly IJsonDataService _jsonService = jsonService;
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;

        #region SET NOTIFICATION CHANNEL
        [Command("set-notification-channel")]
        [Description("set the notification channel to receive bot notifications")]
        public async ValueTask SetNotificationChannel(SlashCommandContext ctx, [Description("channel")] DiscordChannel channel)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var guildResult = await _jsonService.GetGuildFromJsonAsync(ctx.Guild!.Id.ToString());

            if (guildResult.IsOk)
            {
                var chnlId = ulong.Parse(guildResult.Value.NotificationChannelId!);
                var chnl = await ctx.Guild.GetChannelAsync(chnlId);
                if (chnl is { } ch)
                {
                    // Remove the old notification channel
                    guildResult.Value.NotificationChannelId = ch.Guild.GetDefaultChannel()!.Id.ToString();
                    await _jsonService.WriteGuildToJsonAsync(guildResult.Value);
                }
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## 👍SUCCESS👍"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"you will now receive notifications in {channel.Name}\r\nGameday Tracker needs write permissions in any channel you set for notifications"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
            else
            {
                var errorId = Guid.NewGuid().ToString();
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## ❌ FAILURE ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"unable to set {channel.Name} as the notification channel, with error id: {errorId}\r\nError Message {guildResult.Error.ErrorMessage}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
        }
        #endregion

        #region ENABLE/DISABLE REALTIME SCORES
        [Command("enable-realtime-scores")]
        [Description("enable or disable realtime scores")]
        public async ValueTask EnableRealtimeScores(SlashCommandContext ctx, [Description("true : false")] bool enable)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var guildResult = await _jsonService.GetGuildFromJsonAsync(ctx.Guild!.Id.ToString());
            if (guildResult.IsOk)
            {
                guildResult.Value.IsRealTimeScoresEnabled = enable;
                await _jsonService.WriteGuildToJsonAsync(guildResult.Value);
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## 👍SUCCESS👍"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"realtime scores are now {(enable ? "enabled" : "disabled")}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
            else
            {
                var errorId = Guid.NewGuid().ToString();
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## ❌ FAILURE ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"unable to {(enable ? "enable" : "disable")} realtime scores, with error id: {errorId}\r\nError Message {guildResult.Error.ErrorMessage}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
        }
        #endregion

        #region ENABLE/DISABLE HEADLINES

        #endregion

        #region LIST SCHEDULED JOBS
        [Command("list-jobs")]
        [Description("Lists all scheduled jobs for the current guild")]
        public async Task ListScheduledJobs(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobs = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            if (jobs is { } jKeys)
            {
                var sb = new StringBuilder();
                sb.AppendLine("### Scheduled Jobs:");
                foreach (var job in jKeys)
                {
                    var jobDetail = await scheduler.GetJobDetail(job);
                    sb.AppendLine($"- **{jobDetail!.Key.Name}**: {jobDetail.Description}");
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## Scheduled Jobs"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
        }
        #endregion
    }
}
