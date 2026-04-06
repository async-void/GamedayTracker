using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers
{
    public class GuildAddedEventHandler(IJsonDataService jsonService, ILogger<GuildAddedEventHandler> logger) : IEventHandler<GuildCreatedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, GuildCreatedEventArgs args)
        {
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var guild = new Guild()
            {
                GuildId = args.Guild.Id,
                GuildName = args.Guild.Name,
                GuildOwnerId = args.Guild.OwnerId,
                DateAdded = DateTimeOffset.UtcNow,
                IsDailyHeadlinesEnabled = true,
                IsRealTimeScoresEnabled = true,
                ReceiveSystemMessages = true,
                NotificationChannelId = args.Guild.GetDefaultChannel()!.Id.ToString(),
                DiscordMembers = args.Guild.Members.ToDictionary()

            };
            var supportChnl = await sender.GetChannelAsync(1384436855524692048);
            var guilds = sender.Guilds.Values;

            var newChnl = args.Guild.GetDefaultChannel();
            if (newChnl is { } chnl)
            {
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("## Welcome to Gameday Tracker!"),
                                new DiscordSeparatorComponent(true),
                                new DiscordSectionComponent(new DiscordTextDisplayComponent("Use the `help button` to get started!"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "helpId", "Help")),
                                new DiscordSeparatorComponent(true),
                                new DiscordSectionComponent(new DiscordTextDisplayComponent("Headlines and Realtime Scores are enabled by default!"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "settingsId", "Settings")),
                                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                new DiscordSectionComponent(
                                    new DiscordTextDisplayComponent($"Powered by GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var embed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await chnl.SendMessageAsync(embed);
            }
            var guildOwner = await args.Guild.GetMemberAsync(args.Guild.OwnerId);
            await supportChnl.SendMessageAsync(
                $"Guild Added: <t:{unixTimestamp}:R> ``{args.Guild.Name}:({args.Guild.Id}) - Total Guilds: {guilds.Count()}``\r\n" +
                $"``OwnerId: {guildOwner.Id} Owner Membername: {guildOwner.Username}``");
            var guildResult = await jsonService.WriteGuildToJsonAsync(guild);

            if (guildResult.IsOk)
                logger.LogInformation("Guild Added: {guild} ({id}) - Total Guilds: {count}", [args.Guild.Name, args.Guild.Id, guilds.Count()]);
            else
                logger.LogError("unable to save Guild: {guildName} Id: {guildId} to json file | Error: {error}", [args.Guild.Name, args.Guild.Id, guildResult.Error.ErrorMessage]);
        }
    }
}
