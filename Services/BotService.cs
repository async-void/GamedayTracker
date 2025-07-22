using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.Services
{
    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> logger;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly DiscordClient dClient;

        public BotService(ILogger<BotService> logger, IHostApplicationLifetime appLifetime, DiscordClient dClient)
        {
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.dClient = dClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Connecting to Discord...");
            await dClient.ConnectAsync();

            await Task.Delay(1000, cancellationToken);
            var ready = dClient.AllShardsConnected;

            if (ready)
            {
                logger.LogInformation("Connected to Discord successfully.");
                await dClient.UpdateStatusAsync(new DiscordActivity("Watching Scores"), userStatus: DSharpPlus.Entities.DiscordUserStatus.Online);
            }
               
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await dClient.DisconnectAsync();
        }
    }
}
