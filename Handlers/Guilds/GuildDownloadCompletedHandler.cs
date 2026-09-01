using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Guilds
{
    public class GuildDownloadCompletedHandler : IEventHandler<GuildDownloadCompletedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, GuildDownloadCompletedEventArgs eventArgs)
        {
            var count = sender.Guilds.Count;
            await sender.UpdateStatusAsync(new DiscordActivity($"Scores in {count} Servers", DiscordActivityType.Watching));
        }
    }
}
