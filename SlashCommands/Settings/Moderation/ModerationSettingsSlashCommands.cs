using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;

namespace GamedayTracker.SlashCommands.Settings.Moderation
{
    [Command("mod-settings")]
    [Description("moderation settings")]
    public class ModerationSettingsSlashCommands(IConfigurationData configService)
    {
        [Command("notification-channel")]
        [Description("set the notification channel to receive bot notifications")]
        [RequirePermissions(DiscordPermission.ManageGuild)]
        public async ValueTask SetNotificationChannel(CommandContext ctx, [Description("channel id")] DiscordChannel channel)
        {
            await ctx.DeferResponseAsync();
            await using var db = new BotDbContextFactory().CreateDbContext();
            var guildResult = configService.GuildExists(ctx.Guild!);

            if (guildResult.IsOk)
            {
                var guild = db.Guilds.Where(x => (ulong)x.GuildId == ctx.Guild!.Id)!.FirstOrDefault();
                guild!.NotificationChannelId = (long)channel.Id;
                db.Guilds.Update(guild);
                await db.SaveChangesAsync();

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("👍SUCCESS👍")
                        .WithDescription($"you will now receive notifications in {channel.Name}")
                        .WithColor(DiscordColor.Blurple));
                await ctx.RespondAsync(message);
            }
            else
            {
                var errorMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("❗ERROR❗")
                        .WithDescription($"{guildResult.Error.ErrorMessage}")
                        .WithColor(DiscordColor.Blurple));
                await ctx.RespondAsync(errorMessage);
            }
        }
    }
}
