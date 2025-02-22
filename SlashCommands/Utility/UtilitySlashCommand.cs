using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "scoreboardBtn", "Scoreboard"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "standingsBtn", "Standings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "draftBtn", "Draft"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "userSettingsBtn", "User Settings"),
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "newsBtn", "News")
            };

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Blurple)
                    .WithTitle("Help")
                    .WithFooter("Gameday Tracker")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    ).AddComponents(buttons);
            
            await ctx.EditResponseAsync(message);
        }

        [Command("ping")]
        [RequirePermissions(permissions: DiscordPermission.ManageGuild)]
        [Description("get the client latency [must have 'mod' or higher role]")]
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
                    .WithDescription("Total Connection Latency")
                    .AddField("Db Latency", $"{ sw.Elapsed.Humanize()}", true)
                    .AddField("Gateway Latency", $"{connectionLat.Humanize()}", true)
                    .AddField("Uptime", $"{uptime.Duration().Humanize(3, CultureInfo.CurrentCulture, minUnit: TimeUnit.Minute)}", true)
                    .WithColor(DiscordColor.Teal)
                    .WithTimestamp(DateTimeOffset.UtcNow));

            //TODO: finish me.
            _loggerService.Log(LogTarget.Console, LogType.Information, DateTime.UtcNow, "Ping Command Executed");
            await ctx.EditResponseAsync(message);
        }
    }
}
