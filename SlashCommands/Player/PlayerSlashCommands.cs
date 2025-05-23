using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Data;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GamedayTracker.SlashCommands.Player
{
    [Command("player")]
    [Description("Player Commands")]
    public class PlayerSlashCommands(IGameData gameData)
    {
        #region ADD PLAYER TO POOL

        [Command("add")]
        [Description("add player to the pool")]
        public async Task AddPlayer(CommandContext ctx,
            [Parameter("member")] string playerName, [Parameter("company")] string company)
        {
            await ctx.DeferResponseAsync();
            await using var db = new BotDbContextFactory().CreateDbContext();
            
            var player = db.Players.FirstOrDefault(x => x.PlayerName!.Equals(playerName));
            if (player is null)
            {

                var newPlayer = new PoolPlayer
                {
                    PlayerName = playerName,
                    Company = company,
                };
                db.Players.Add(newPlayer);
                await db.SaveChangesAsync();

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"Player Added {playerName} Successfully"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"run command again to add more players to the pool!"),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}"), 
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate", false, new DiscordComponentEmoji("\ud83d\udcb5")))
                ];

                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(new DiscordContainerComponent(components, true, DiscordColor.Green));
                await ctx.EditResponseAsync(message);
            }

            
        }

        #endregion

    }
}
