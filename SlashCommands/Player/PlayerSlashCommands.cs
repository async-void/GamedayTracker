using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands(IPlayerData playerService)
    {
        #region ADD PLAYER TO POOL

        [Command("add")]
        [Description("add player to the pool")]
        public async Task AddPlayer(MessageCommandContext ctx,
            [Parameter("Name")] string playerName, [Parameter("Company")] string company, [Parameter("Balance")] string balance)
        {
            await ctx.DeferResponseAsync();
            var player = new PoolPlayer
            {
                PlayerName = playerName,
                Company = company,
                Balance = double.TryParse(balance, out var bal) ? bal : 0.0,
                DepositTimestamp = DateTime.UtcNow
            };
            var result = await playerService.WritePlayerToXmlAsync(player);
            
            if (result.IsOk)
            {
                DiscordComponent[] buttons =
                [
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
                ];
                
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("👍**Player Added Successfully**"),
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
    }

        #endregion
}
