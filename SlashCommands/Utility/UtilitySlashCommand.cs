using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Data;
using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using Humanizer;
using Humanizer.Localisation;

namespace GamedayTracker.SlashCommands.Utility
{
   
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand(ITimerService timerService, ILogger loggerService)
    {
        private readonly ITimerService _timerService = timerService;
        private readonly ILogger _loggerService = loggerService;

        [Command("help")]
        [Description("a list of commands and a brief explaination")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var buttons = new DiscordComponent[]
            {
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "scoreboardHelpBtn", "Scoreboard"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "standingsHelpBtn", "Standings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "draftHelpBtn", "Draft"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "userSettingsHelpBtn", "User Settings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "newsHelpBtn", "News")
            };

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Blurple)
                    .WithTitle("Help")
                    .WithFooter("Gameday Tracker")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    ).AddComponents(buttons);
            _loggerService.Log(LogTarget.Console, LogType.Information, DateTimeOffset.UtcNow, "Help slash command was called!");
            await ctx.EditResponseAsync(message);
        }

        [Command("ping")]
        [RequirePermissions(permissions: DiscordPermission.ManageGuild)]
        [Description("get the client latency [must have mod or higher roles]")]
        public async ValueTask Ping(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var sw = new Stopwatch();
            sw.Start();
            await using var db = new AppDbContextFactory().CreateDbContext();
            sw.Stop();
            var guildId = ctx.Guild!.Id;
            var connectionLat = ctx.Client.GetConnectionLatency(guildId);
            var uptime = _timerService.CalculateRunningTime();
            var message = new DiscordMessageBuilder().AddEmbed(
                new DiscordEmbedBuilder()
                    .WithTitle("Latency")
                    .AddField("Db", sw.Elapsed.Humanize(), true)
                    .AddField("Discord", connectionLat.Humanize(), true)
                    .AddField("Lifetime", uptime.Humanize(), true)
                    .WithColor(DiscordColor.Teal)
                    .WithTimestamp(DateTimeOffset.UtcNow));

            //TODO: finish me.
            _loggerService.Log(LogTarget.Console, LogType.Information, DateTime.UtcNow, "Ping Command Executed");
            await ctx.EditResponseAsync(message);
        }
    }
}
