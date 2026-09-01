using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GamedayTracker.Data;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl.Matchers;
using CommandAttribute = DSharpPlus.Commands.CommandAttribute;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using RequirePermissionsAttribute = DSharpPlus.Commands.ContextChecks.RequirePermissionsAttribute;

namespace GamedayTracker.SlashCommands.Settings.Moderation
{
    [Command("moderation")]
    [Description("Moderation Slash Commands")]
    [RequirePermissions([DiscordPermission.Administrator, DiscordPermission.ManageGuild])]
    public class ModerationSettingsSlashCommands(IJsonDataService jsonService, ISchedulerFactory schedulerFactory, IDbContextFactory<BotDbContext> dbFactory)
    {
        private readonly IJsonDataService _jsonService = jsonService;
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;

        #region SET NOTIFICATION CHANNEL
        [Command("set-notification-channel")]
        [System.ComponentModel.Description("set the notification channel to receive bot notifications")]
        public async ValueTask SetNotificationChannel(SlashCommandContext ctx, [Description("channel")] DiscordChannel channel)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var guildResult = await _jsonService.GetGuildFromJsonAsync(ctx.Guild!.Id);

            if (guildResult.IsOk)
            {
                guildResult.Value.NotificationChannelId = channel.Id.ToString();
                var notifyResult = await _jsonService.UpdateGuildDataAsync(guildResult.Value);

                if (notifyResult.IsOk)
                {
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
        [Command("toggle-realtime-scores")]
        [Description("enable or disable realtime scores")]
        public async ValueTask EnableRealtimeScores(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            using var db = dbFactory.CreateDbContext();
            var guild = db.Guilds?.Where(x => x.GuildId == ctx.Guild!.Id).FirstOrDefault();

            if (guild is not null)
            {
                guild.IsRealTimeScoresEnabled = !guild.IsRealTimeScoresEnabled;
                var isEnabled = guild.IsRealTimeScoresEnabled;
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## 👍SUCCESS👍"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent($"realtime scores are now {(isEnabled ? "enabled" : "disabled")}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ {timestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
                db.Update(guild);
                await db.SaveChangesAsync();
            }
            else
            {
                var errorId = Guid.NewGuid().ToString();
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## ❌ FAILURE ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"unable to update realtime scores, please try again later!"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{timestamp}:F>"),
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
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobs = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            if (jobs is { } jKeys)
            {
                var sb = new StringBuilder();
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
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ {unixTimestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
        }
        #endregion

        #region GET DAILY NUMBER PICKS FOR USER|GUILD|DATE
        [Command("get-user-lottery-picks")]
        [Description("get the daily lottery picks for a user, guild, and date")]
        [RequireRoles(RoleCheckMode.MatchNames, "Owner")]
        public async Task GetUserLotteryPicks(SlashCommandContext ctx, [Parameter("user")] DiscordUser user, [Parameter("date")] string date)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            if (DateOnly.TryParseExact(date, "MM-dd-yyyy", out var dateParsed))
            {
                 var userNumbers = await _jsonService.GetUserDailyNumbersFromJsonAsync(ctx.Guild!.Id, user.Id, dateParsed);
                if (userNumbers.Any())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"**Lottery Picks for {user.Username} on {dateParsed}**");
                    sb.AppendLine();
                    foreach (var pick in userNumbers.Take(5))
                    {
                        sb.AppendLine($"- Numbers: {string.Join(", ", pick.Numbers)}");
                        sb.AppendLine($"- Timestamp: {pick.Timestamp}");
                    }

                    DiscordComponent[] components =
                    [
                        new DiscordTextDisplayComponent(sb.ToString()),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ {unixTimestamp}"),
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
                    DiscordComponent[] components =
                    [
                       new DiscordTextDisplayComponent($"Daily Numbers for **{user.Username} for date: {date:MM-dd-yyy tt}** Not Found"),
                       new DiscordTextDisplayComponent($"This could be because the user did not submit picks for this date, " +
                                                       $"or there was an error retrieving the data\r\nPlease verify the user and date and try again\r\n" +
                                                       $"date format: 03-10-2026"),
                       new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ {unixTimestamp}"),
                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                    ];
                    var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                    var message = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(container);
                    await ctx.RespondAsync(message);
                }
            }
            else
            {
                await ctx.RespondAsync("Invalid date, use MM-dd-yyyy");
                return;
            }
        }
        #endregion

        #region PURGE CHANNEL MESSAGES
        [Command("purge")]
        [Description("purge messages in a channel, with an optional limit (default 100)")]
        public static async ValueTask PurgeChannelMessages(SlashCommandContext ctx, [Parameter("Amount")] int limit = 100)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            try
            {
                var messages = ctx.Channel.GetMessagesAsync(limit);
                var count = 0;
                await foreach (var msg in messages)
                {
                    await ctx.Channel.DeleteMessageAsync(msg);
                    await Task.Delay(200);
                    ++count;
                }

                var label = count == 1 ? "message" : "messages";
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## 👍SUCCESS👍"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"purged {count} {label} in {ctx.Channel.Name}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ {unixTimestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(message);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## ❌ FAILURE ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"unable to purge messages in {ctx.Channel.Name}, with error id: {errorId}\r\nError Message {ex.Message}"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ {unixTimestamp}"),
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
    }
}
