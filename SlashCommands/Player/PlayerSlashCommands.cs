using System.ComponentModel;
using System.Linq;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands(IJsonDataService jsonDataService)
    {
        #region ADD PLAYER TO POOL

        [Command("add")]
        [Description("add player to the pool")]
        [RequirePermissions(DiscordPermission.ManageGuild)]
        public async Task AddPlayer(MessageCommandContext ctx, 
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
        public async ValueTask GetPlayerProfile(CommandContext ctx, [Parameter("Member")] DiscordMember dMember)
        {
            await ctx.DeferResponseAsync();

            var result = await jsonDataService.GetMemberFromJsonAsync(dMember.Id.ToString());
            DiscordComponent[] components;
            DiscordContainerComponent container;

            if (result.IsOk)
            {
                var member = result.Value;
                components = [
                    new DiscordTextDisplayComponent($"**{member.MemberName}**'s Profile"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Balance: {member.Balance:C}"),
                    new DiscordTextDisplayComponent($"Last Deposit: {member.LastDeposit?.ToString("g") ?? "Never"}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Guilds: {string.Join(",", member.Guilds!)}"),
                    new DiscordTextDisplayComponent($"Favorite Team: {member.FavoriteTeam ?? "None"}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTimeOffset.UtcNow.ToString("f")}"),
                                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
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

                }

                await jsonDataService.WriteMemberToJsonAsync(new GuildMember
                {
                    Id = Guid.NewGuid(),
                    MemberId = dMember.Id.ToString(),
                    MemberName = dMember.Username,
                    Guilds = [ new Guild { Id = Guid.NewGuid(), 
                                                GuildId = (long)dMember.Guild.Id, 
                                                GuildName = dMember.Guild.Name } ],
                    Balance = 0
                });

                var m = await jsonDataService.GetMemberFromJsonAsync(dMember.Id.ToString());

                if (m.IsOk)
                {
                    components = [
                    new DiscordTextDisplayComponent($"**{m.Value.MemberName}**'s Profile"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Balance: {m.Value.Balance:C}"),
                    new DiscordTextDisplayComponent($"Last Deposit: {m.Value.LastDeposit?.ToString("g") ?? "Never"}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Guilds: {string.Join(",", m.Value.Guilds!)}"),
                    new DiscordTextDisplayComponent($"Favorite Team: {m.Value.FavoriteTeam ?? "None"}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTimeOffset.UtcNow.ToString("f")}"),
                                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                    container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                    var msg = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(container);
                    await ctx.RespondAsync(msg);
                }
                else
                {
                    components = [
                        new DiscordTextDisplayComponent($"## ERROR"),
                        new DiscordSeparatorComponent(true),
                        new DiscordTextDisplayComponent($"Balance: {m.Error.ErrorMessage}"),
                        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                        new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTimeOffset.UtcNow.ToString("f")}"),
                                                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                    ];
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


    

