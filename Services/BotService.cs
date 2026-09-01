using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Utility.Ansi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.Services
{
    public class BotService(ILogger<BotService> logger, IHostApplicationLifetime appLifetime, DiscordClient dClient) : IHostedService
    {
        private readonly ILogger<BotService> _logger = logger;
        private readonly IHostApplicationLifetime _appLifetime = appLifetime;
        private readonly DiscordClient _dClient = dClient;

      
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Connecting to {AnsiColors.GetAnsiCode("orange")}Discord...");
            await _dClient.ConnectAsync(new DiscordActivity($"Scores", DiscordActivityType.Watching), DiscordUserStatus.Online);
            
            string[] handlers = [ "MemberBetsPaginationHandler", "NFLScoreboardPaginationHandler", "TeamStatsPaginationHandler" ];
            _logger.LogInformation($"Registering Pagination Handlers [{AnsiColors.GetAnsiCode("orange")} {string.Join(",", handlers)}]");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _dClient.DisconnectAsync();
        }
    }
}
