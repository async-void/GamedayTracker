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
using Serilog;
using ILogger = GamedayTracker.Interfaces.ILogger;

namespace GamedayTracker.SlashCommands.Utility
{
   
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand(IBotTimer botTimer, ILogger loggerService)
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
            Log.Information("Help slash command was called!");
            //loggerService.Log(LogTarget.Console, LogType.Information, DateTime.UtcNow, "Help slash command was called!");
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
            
            var guildId = ctx.Guild!.Id;
            var connectionLat = ctx.Client.GetConnectionLatency(guildId);
            var timestamp = DateTime.UtcNow;
            var savedTimeStamp = await botTimer.GetTimestampFromTextAsync();
            sw.Stop();
            if (savedTimeStamp.IsOk)
            {
                var uptime = timestamp - savedTimeStamp.Value; 
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"Latency **{sw.Elapsed.Humanize()}** "),
                    new DiscordTextDisplayComponent($"Discord **{connectionLat.Humanize()}**"),
                    new DiscordTextDisplayComponent($"Uptime **{uptime.Humanize(3, maxUnit: TimeUnit.Year, minUnit: TimeUnit.Second)}**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"GamedayTracker ©️ {savedTimeStamp.Value:MM-dd-yyyy hh:mm:ss tt zzz}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                ];
                DiscordContainerComponent container = new(components, false, DiscordColor.Blurple);

                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                Log.Information("Ping Command Executed");
                await ctx.EditResponseAsync(message);
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordMessageBuilder()
                    .WithContent(savedTimeStamp.Error.ErrorMessage!));
                Log.Error(savedTimeStamp.Error.ErrorMessage!);

            }
  
        }
        #endregion
    }
}
