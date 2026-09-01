using DSharpPlus;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Guilds
{
    public class GuildDeletedEventHandler(ILogger<GuildDeletedEventHandler> logger) : IEventHandler<GuildDeletedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, GuildDeletedEventArgs args)
        {
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var jsonService = sender.ServiceProvider.GetRequiredService<IJsonDataService>();
            var guildResult = await jsonService.GetGuildFromJsonAsync(args.Guild.Id);
            if (guildResult.IsOk)
            {
                var removedResult = await jsonService.RemoveGuildDataAsync(guildResult.Value.GuildId);
                var guildOwner = await args.Guild.GetMemberAsync(args.Guild.OwnerId);
                if (removedResult.IsOk)
                {
                    await guildOwner.SendMessageAsync(
                        $"``Gameday Tracker has been removed at: [{unixTimestamp}]``\r\nyou will no longer be receiving Daily Headlines or Realtime Scores Updates");
                    logger.LogInformation("GamedayTracker has been removed from: {guild}", args.Guild.Name);
                }
                else
                {
                    await args.Guild.GetDefaultChannel()!.SendMessageAsync(
                        $"``Error removing Gameday Tracker from {args.Guild.Name} ({args.Guild.Id}) at: [{unixTimestamp}]``\r\n{removedResult.Error.ErrorMessage}");
                    logger.LogError("Error removing guild {Name} {Id}: \r\nError: {error}", args.Guild.Name, args.Guild.Id, removedResult.Error.ErrorMessage);
                }

            }
        }
    }
}
