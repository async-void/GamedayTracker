using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GamedayTracker.Jobs
{
    public class UpdateBotStatusJob(DiscordClient client, ILogger<UpdateBotStatusJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var guildCount = client.Guilds.Count;
            var userCount = client.Guilds.Values.Sum(g => g.Members.Count);
            var statuses = new string[]
            {
                $"Watching Scores for {guildCount} Servers",
                $"Watching Games for {guildCount} Servers",
                $"Watching Bets for {guildCount} Servers",
                $"Watching News for {guildCount} Servers",
                $"Serving {userCount} members"
            };
            var index = new Random().Next(statuses.Length);
            var statusMessage = $"{statuses[index]}";
            logger.LogInformation("Updating Bot Status...");
            await client.UpdateStatusAsync(new DiscordActivity($"{statusMessage}", DiscordActivityType.Custom));
            logger.LogInformation($"Updating user count | {userCount} members...");
            logger.LogInformation("Bot Status Update Complete!...");    
        }
    }
}
