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

namespace GamedayTracker.SlashCommands.Utility
{
   
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand(ITimerService timerService, ILogger loggerService)
    {

        [Command("help")]
        [Description("help commands and a brief explaination")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            DiscordComponent[] buttons =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "scoreboardHelpBtn", "Scoreboard"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "standingsHelpBtn", "Standings"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "draftHelpBtn", "Draft"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "userSettingsHelpBtn", "User Settings"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "newsHelpBtn", "News")
            ];

            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent("Help Section"),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent("below is a list of buttons where you will select a button to get the desired help section."),
                new DiscordActionRowComponent(buttons)
                
            ];

            var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
            var message = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            loggerService.Log(LogTarget.Console, LogType.Information, DateTimeOffset.UtcNow, "Help slash command was called!");
            await ctx.EditResponseAsync(message);
        }

        #region PING
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
            var uptime = timerService.CalculateRunningTime();
            
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"DB **{sw.Elapsed.Humanize()}** "),
                new DiscordTextDisplayComponent($"Discord **{connectionLat.Humanize()}**"),
                new DiscordTextDisplayComponent($"Lifetime **{uptime.Humanize()}**")
            ];
            DiscordContainerComponent container = new(components, false, DiscordColor.Blurple);

            var message = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
                

            //TODO: finish me.
            loggerService.Log(LogTarget.Console, LogType.Information, DateTime.UtcNow, "Ping Command Executed");
            await ctx.EditResponseAsync(message);
        }
        #endregion
    }
}
