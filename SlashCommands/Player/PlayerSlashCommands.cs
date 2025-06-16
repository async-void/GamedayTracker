using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
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
            //        new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),

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
            //        new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),

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

            if (result.IsOk)
            {
                var member = result.Value;

                var embed = new DiscordEmbedBuilder().WithTitle($"Player Profile: {member.MemberName}")
                    .AddField("ID", member.Id.ToString(), true)
                    .AddField("Member ID", member.MemberId.ToString(), true)
                    .AddField("Guild ID", member.GuildId.ToString(), true)
                    .WithColor(DiscordColor.Blurple);

                await ctx.EditResponseAsync(new DiscordMessageBuilder().AddEmbed(embed));
            }
            else
            {
                var error = result.Error;

                var embed = new DiscordEmbedBuilder().WithTitle("⚠️ Error")
                    .WithDescription(error.ErrorMessage ?? "An unknown error occurred.")
                    .WithColor(DiscordColor.Red)
                    .WithFooter($"Gameday Tracker ©️ {DateTime.UtcNow.ToShortTimeString()}");

                await ctx.EditResponseAsync(new DiscordMessageBuilder().AddEmbed(embed));
            }
        }
        #endregion
    }
}


    

