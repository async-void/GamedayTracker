using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Session
{
    public class SessionResumedEventHandler(ILogger<SessionResumedEventHandler> logger): IEventHandler<SessionResumedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, SessionResumedEventArgs eventArgs)
        {
            var guildCount = sender.Guilds.Count;
            var userCount = sender.Guilds.Values.Sum(g => g.Members.Count);
            var statusMessage = $"Serving {guildCount} Servers";
            await sender.UpdateStatusAsync(new DiscordActivity($"Scores in {guildCount} Servers", DiscordActivityType.Watching));
            var logChnl = await sender.GetChannelAsync(1384436855524692048);
            await logChnl.SendMessageAsync(
                $"Updated bot status: {statusMessage} with a total of ``{userCount}`` users.");
            logger.LogInformation("Session Resumed with Status: {status}", statusMessage);
        }
    }
}
