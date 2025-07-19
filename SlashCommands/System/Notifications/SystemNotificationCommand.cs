
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Attributes;
using GamedayTracker.Interfaces;
using Serilog;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.System.Notifications
{
    public class SystemNotificationCommand(IJsonDataService jsonDataService)
    {
        [Command("notify")]
        [Description("Sends a system notification to all Guilds.")]
        [RequirePermissions(DiscordPermission.Administrator)]   
        public async Task NotifyAsync(CommandContext ctx, [Parameter("The message to send.")] string message)
        {
            await ctx.DeferResponseAsync();
            var author = ctx.User;

            if (author.Id != 524434302361010186)
            {
                var errMsg = new DiscordInteractionResponseBuilder()
                    .WithContent("You do not have permission to use this command.")
                    .AsEphemeral(true);
                await ctx.EditResponseAsync(errMsg);
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
                        if (chnl is { } channel)
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
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        Log.Information($"Failed to send message to guild {guild.Id}: {ex.Message}");
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
                var errMessage = new DiscordInteractionResponseBuilder()
                   .WithContent($"Failed to retreive Guilds")
                   .AsEphemeral(true);
                await ctx.EditResponseAsync(errMessage);
            }

        }
    }
}
