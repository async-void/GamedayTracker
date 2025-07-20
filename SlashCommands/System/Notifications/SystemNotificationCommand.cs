
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Utility;
using Serilog;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.System.Notifications
{
    [SlashCommandGroup("notification", "notification commands")]
    public class SystemNotificationCommand(IJsonDataService jsonDataService)
    {
        [SlashCommand("notify", "Sends a system notification to all guilds that have ReceiveNotifications flag set to true.")]
        public async Task NotifyAsync(SlashCommandContext ctx, [Parameter("The message to send.")] string message)
        {
            await ctx.DeferResponseAsync();
            var userId = ctx.User.Id;

            if (userId != 524434302361010186)
            {
                var errMessage = new DiscordInteractionResponseBuilder()
                    .WithContent("this command is reserved for GamedayTracker devs. you donot have permission to execute!")
                    .AsEphemeral(true);
                await ctx.EditResponseAsync(errMessage);
                return;
            }
            var guildResult = await jsonDataService.GetGuildsFromJsonAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (guildResult.IsOk)
            {
                var guilds = guildResult.Value;
                foreach (var guild in guilds)
                {
                    try
                    {
                        var chnl = await ctx.Client.GetChannelAsync(ulong.Parse(guild.NotificationChannelId!));
                        if (chnl is { } channel && guild.ReceiveSystemMessages == true)
                        {
                            DiscordComponent[] components =
                            [
                                new DiscordTextDisplayComponent("### System Notification 📝"),
                                new DiscordSeparatorComponent(true),
                                new DiscordTextDisplayComponent($"{message}"),
                                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                            ];

                            var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
                            var msg = new DiscordMessageBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(container);
                            await chnl.SendMessageAsync(msg);
                        }
                        else
                            Log.Information($"Guild [{guild.GuildName}] not found! or system messages are disabled.");
                    }
                    catch (Exception ex)
                    {
                        Log.Information($"Failed to send message to guild {guild.GuildName}: {ex.Message}");
                        continue;
                    }
                }
                var dMsg = new DiscordInteractionResponseBuilder()
                  .WithContent($"Notification sent to {guilds.Count} guilds.")
                  .AsEphemeral(true);
                await ctx.EditResponseAsync(dMsg);
            }
            else
            {
                //await foreach (var g in ctx.Client.GetGuildsAsync())
                //{
                    //TODO: eventially I will send the message to all guilds in the client cashe!
                //}

                var errMessage = new DiscordInteractionResponseBuilder()
                   .WithContent($"{SystemErrorCodes.GetErrorMessage(ErrorCode.GuildNotFound)} With Error Code: GDT-{ErrorCode.GuildNotFound}")
                   .AsEphemeral(true);
                await ctx.EditResponseAsync(errMessage);
            }

        }
    }
}
