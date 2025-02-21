using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using GamedayTracker.Data;
using GamedayTracker.Factories;

namespace GamedayTracker.SlashCommands.Help
{
    [Command("utility")]
    [Description("Utility Slash Commands")]
    public class UtilitySlashCommand
    {
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

            var message = new DiscordMessageBuilder().AddEmbed(
                new DiscordEmbedBuilder()
                    .WithTitle("Latency")
                    .WithDescription("Total Connection Latency")
                    .AddField("Db Latency", $"{ sw.ElapsedMilliseconds}ms", true)
                    .AddField("Gateway Latency", $"{connectionLat}ms", true)
                    .WithColor(DiscordColor.Teal)
                    .WithTimestamp(DateTimeOffset.UtcNow));

            //TODO: finish me.
            await ctx.EditResponseAsync(message);
        }
    }
}
