using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Utility.Ansi;
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
            logger.LogInformation($"Connecting to {AnsiColors.GetAnsiCode("orange")}Discord...");
            await dClient.ConnectAsync(new DiscordActivity($"Scores", DiscordActivityType.Watching), DiscordUserStatus.Online);
            string[] handlers = [ "MemberBetsPaginationHandler", "NFLScoreboardPaginationHandler", "TeamStatsPaginationHandler" ];
            logger.LogInformation($"Registering Pagination Handlers [{AnsiColors.GetAnsiCode("orange")} {string.Join(",", handlers)}]");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await dClient.DisconnectAsync();
        }
    }
}
