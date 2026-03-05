using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class UpdateBotStatusJob(DiscordClient client, ILogger<UpdateBotStatusJob> logger) : IJob
    {
        private readonly DiscordClient _client = client;
        public async Task Execute(IJobExecutionContext context)
        {
            
            var guildCount = _client.Guilds.Count;
            var userCount = _client.Guilds.Values.Sum(g => g.Members.Count);
            var statuses = new string[]
            {
                $"Watching Scores for {guildCount} Servers",
                $"Watching Games for {guildCount} Servers",
                $"Watching Bets for {guildCount} Servers",
                $"Watching News for {guildCount} Servers",
            };
            var index = new Random().Next(statuses.Length);
            var statusMessage = $"{statuses[index]}";
            await _client.UpdateStatusAsync(new DiscordActivity($"{statusMessage}", DiscordActivityType.Custom));
            logger.LogInformation("Updating Bot Status...");
            //var logChnl = await _client.GetChannelAsync(1384436855524692048);
            //await logChnl.SendMessageAsync(
            //    $"Updated bot status: Watching {statusMessage} with a total of ``{userCount}`` users.");    
        }
    }
}
