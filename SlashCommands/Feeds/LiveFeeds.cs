using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.SlashCommands.Feeds
{
    public class LiveFeeds
    {
        [Command("live-feeds")]
        [Description("Get live feeds for Scores, Standings and Daily Headlines.")]
        public async ValueTask GetLiveFeeds(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            DiscordComponent[] btns =
            [
                new DiscordLinkButtonComponent("https://discord.com/channels/1384428811805921301/1398021337498390539", "Live Scores"),
                new DiscordLinkButtonComponent("https://discord.com/channels/1384428811805921301/1398021268032196698", "Daily Headlines"),
                new DiscordLinkButtonComponent("https://discord.com/channels/1384428811805921301/1398735401048608960", "Daily Standings")
            ];
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent("## Live Feeds\r\n-# live feeds for Scores, Standings and Headlines"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent("Use the links below, and \"Follow\" the channels to get live scores, " +
                "daily headlines and standings in your server.\r\n-# Note: Once you follow the channel, " +
                "you will only get the messages on the next day/week due to how announcement channels work."),
                new DiscordSeparatorComponent(true),
                new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem("https://i.imgur.com/10HHPHh.png")),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordActionRowComponent(btns),
                 new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            ];
            var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            var message = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            await ctx.EditResponseAsync(message);
        }
    }
}
