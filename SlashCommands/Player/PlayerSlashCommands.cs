using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Humanizer;
using Serilog;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Log = Serilog.Log;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands(IJsonDataService jsonDataService)
    {
        #region ADD PLAYER TO POOL

        [Command("add")]
        [Description("add player to the pool")]
        [RequirePermissions(permissions: DiscordPermission.ManageGuild)]
        public async Task AddPlayer(SlashCommandContext ctx, 
            [Parameter("Name")] string playerName, [Parameter("Company")] string company,
            [Parameter("Balance")] string balance)
        {
            await ctx.DeferResponseAsync();
            var playerId = await jsonDataService.GeneratePlayerIdAsync();
            var playerIdentifier = jsonDataService.GeneratePlayerIdentifier();

            //TODO: re-write this to add a PoolPlayer to json instead of GuildMember!

            // var result = await jsonDataService.WriteMemberToJsonAsync(player);

            //if (result.IsOk)
            //{
            //    DiscordComponent[] buttons =
            //    [
            //        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            //    ];

            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent("👍 **Player Added Successfully**"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordActionRowComponent(buttons),
            //        new DiscordTextDisplayComponent($"Gameday Tracker - {DateTimeOffset.UtcNow.ToLongDateString()}"),

            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            //    var msg = new DiscordMessageBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);

            //    await ctx.EditResponseAsync(msg);
            //}
            //else
            //{
            //    DiscordComponent[] buttons =
            //    [
            //        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
            //    ];

            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent("⚠️ **UH-OH**"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordTextDisplayComponent($"{result.Error.ErrorMessage}"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordActionRowComponent(buttons),
            //        new DiscordTextDisplayComponent($"Gameday Tracker - {DateTimeOffset.UtcNow.ToLongDateString()}"),

            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
            //    var msg = new DiscordMessageBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);

            // await ctx.EditResponseAsync(msg);
            // }
        }
        #endregion

        #region PLAYER PROFILE
        [Command("profile")]
        [Description("View player profile")]//TODO: fix me
        public async ValueTask GetPlayerProfile(SlashCommandContext ctx, [Parameter("Member")] DiscordMember dMember)
        {
            await ctx.DeferResponseAsync();

            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var result = await jsonDataService.GetMemberFromJsonAsync(dMember.Id.ToString(), dMember.Guild.Id.ToString());
            DiscordComponent[] components;
            DiscordContainerComponent container;

            if (result.IsOk)
            {
                var member = result.Value;
                var guild = await ctx.Client.GetGuildAsync(dMember.Guild.Id);
                var roles = dMember.Roles.Select(r => r.Mention).ToList();
                var memRoles = new List<string>();
                var favTeam = member.FavoriteTeam?.Titleize() ?? "None";
                var lastDeposit = member.LastDeposit?.ToString("g") ?? "Never";
                var wins = member.BetWins.ToString();

                var sb = new StringBuilder();
               
                sb.AppendLine($"**Member Name:** {dMember.Username}");
                sb.AppendLine($"**Member ID:** {dMember.Id}");
                sb.AppendLine($"**Joined:** {dMember.JoinedAt.Humanize()}");

                components = 
                [
                    new DiscordTextDisplayComponent($"**{dMember.Username}**'s Profile"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent(sb.ToString()),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Roles: {string.Join(" ", roles)}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Balance: {member.Balance:C}"),
                    new DiscordTextDisplayComponent($"Last Deposit: {lastDeposit}"),
                    new DiscordTextDisplayComponent($"Bet Wins: {wins}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Favorite Team: {favTeam}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                Log.Information($"Player Exists, profile found for - {member.MemberName}");
                container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(msg);
            }
            else
            {
                if (result.Error.ErrorType.Equals(ErrorType.FATAL))
                {
                    Log.Error(result.Error.ErrorMessage!);
                    return;
                }

                await jsonDataService.WriteMemberToJsonAsync(new GuildMember
                {
                    Id = Guid.NewGuid(),
                    GuildId = dMember.Guild.Id.ToString(),
                    MemberId = dMember.Id.ToString(),
                    MemberName = dMember.Username,
                    Balance = 100,
                    BetWins = 0,
                });

                var m = await jsonDataService.GetMemberFromJsonAsync(dMember.Id.ToString(), dMember.Guild.Id.ToString());

                if (m.IsOk)
                {
                    var sb = new StringBuilder();
                    var roles = dMember.Roles.Select(r => r.Mention).ToList();
                    var favTeam = m.Value.FavoriteTeam?.Titleize() ?? "None";
                    var lastDeposit = m.Value.LastDeposit?.ToString("g") ?? "Never";

                    components =
                   [
                        new DiscordTextDisplayComponent($"**{m.Value.MemberName}**'s Profile"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"Roles: {string.Join(",", roles)}"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"Balance: {m.Value.Balance:C}"),
                        new DiscordTextDisplayComponent($"Last Deposit: {lastDeposit}"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"Favorite Team: {favTeam}"),
                        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                   ];
                    Log.Information($"Player profile found for - {m.Value.MemberName}");
                    container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                    var msg = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(container);
                    await ctx.RespondAsync(msg);
                }
                else
                {
                    components = 
                    [
                        new DiscordTextDisplayComponent($"## ❌ ERROR ❌"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"{m.Error.ErrorMessage}"),
                        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                    ];
                    Log.Warning($"Error: {m.Error.ErrorMessage} in Guild: {ctx.Guild!.Name}");
                    container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
                        var msg = new DiscordMessageBuilder()
                            .EnableV2Components()
                            .AddContainerComponent(container);
                        await ctx.RespondAsync(msg);
                    }
            }
        }
        #endregion
    }
}


    

