using DSharpPlus;
using DSharpPlus.Entities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class UpdateBotStatusJob(DiscordClient client) : IJob
    {
        private readonly DiscordClient _client = client;
        public async Task Execute(IJobExecutionContext context)
        {
            
            var guildCount = _client.Guilds.Count;
            var userCount = _client.Guilds.Values.Sum(g => g.Members.Count);
            var statuses = new string[]
            {
                $" Scores for {guildCount} Servers",
                $" Games for {guildCount} Servers",
                $" Bets for {guildCount} Servers",
                $" News for {guildCount} Servers",
            };
            var index = new Random().Next(statuses.Length);
            var statusMessage = $"{statuses[index]}";
            await _client.UpdateStatusAsync(new DiscordActivity($"{statusMessage}", DiscordActivityType.Watching));
            //var logChnl = await _client.GetChannelAsync(1384436855524692048);
            //await logChnl.SendMessageAsync(
            //    $"Updated bot status: Watching {statusMessage} with a total of ``{userCount}`` users.");    
        }
    }
}
