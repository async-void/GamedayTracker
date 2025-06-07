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
    public class PlayerSlashCommands(IPlayerData playerService, IJsonDataService jsonDataService)
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

            var player = new PoolPlayer
            {
                Id = playerId.Value,
                PlayerId = playerIdentifier.Value,
                PlayerName = playerName,
                Company = company,
                Balance = double.TryParse(balance, out var bal) ? bal : 0.0,
                DepositTimestamp = DateTime.UtcNow
            };
            //var result = await playerService.WritePlayerToXmlAsync(player);
            var result = await jsonDataService.WritePlayerToJsonAsync(player);

            if (result.IsOk)
            {
                DiscordComponent[] buttons =
                [
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
                ];

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("👍 **Player Added Successfully**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordActionRowComponent(buttons),
                    new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),

                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.EditResponseAsync(msg);
            }
            else
            {
                DiscordComponent[] buttons =
                [
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
                ];

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("⚠️ **UH-OH**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{result.Error.ErrorMessage}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordActionRowComponent(buttons),
                    new DiscordTextDisplayComponent($"Gameday Tracker - {DateTime.UtcNow.ToLongDateString()}"),

                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.EditResponseAsync(msg);
            }
        }
        #endregion


        #region PLAYER PROFILE
        [Command("profile")]
        [Description("View player profile")]
        public async ValueTask GetPlayerProfile(CommandContext ctx, [Parameter("Player")] string playerName)
        {
            await ctx.DeferResponseAsync();

            var result = await playerService.GetPlayerFromXmlAsync(playerName);

            if (result.IsOk)
            {
                var player = result.Value;

                var embed = new DiscordEmbedBuilder().WithTitle($"Player Profile: {player.PlayerName}")
                    .AddField("ID", player.Id.ToString(), true)
                    .AddField("Player ID", player.PlayerId.ToString(), true)
                    .AddField("Company", player.Company ?? "N/A", true)
                    .AddField("Balance", player.Balance.ToString("C"), true)
                    .AddField("Deposit Timestamp", player.DepositTimestamp.ToString("g"), true)
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


    

