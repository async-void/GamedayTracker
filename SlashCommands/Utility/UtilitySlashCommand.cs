﻿using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Humanizer;
using Serilog;

namespace GamedayTracker.SlashCommands.Utility
{
   
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand(IBotTimer botTimer)
    {
        #region HELP
        [Command("help")]//TODO: fix me
        [Description("help commands and a brief explaination")]
        public async Task Help(SlashCommandContext ctx)
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
        #endregion

        #region PING
        [Command("ping")]
        [Description("get the client latency [must have mod or higher roles]")]
        public async ValueTask Ping(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"GamedayTracker ©️ <t:{unixTimestamp}:F>"),
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

        #region ABOUT
        [Command("about")]
        [Description("get information about the bot")]
        public async ValueTask About(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var bot = ctx.Client.CurrentUser;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = DateTimeOffset.UtcNow;
            var aboutText = new StringBuilder()
                .AppendLine($"**Version:** {version}")
                .AppendLine($"**Build Date:** {buildDate:MM-dd-yyyy}")
                .AppendLine($"**Guilds:** {ctx.Client.Guilds.Count}")
                .AppendLine("**Created by:** <@524434302361010186>")
                .AppendLine("")
                .AppendLine("[Support Server](https://discord.gg/r65JVSs4)")
                .AppendLine("[GitHub](https://github.com/async-void/GamedayTracker)");

            DiscordComponent[] components =
            [
                new DiscordSectionComponent(new DiscordTextDisplayComponent("GamedayTracker"),
                    new DiscordThumbnailComponent($"{bot.AvatarUrl}")),
                new DiscordSeparatorComponent(true),
                new DiscordTextDisplayComponent(aboutText.ToString()),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            ];
            var container = new DiscordContainerComponent(components, false, DiscordColor.Goldenrod); 
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            await ctx.EditResponseAsync(msg);
        }
        #endregion
    }
}
