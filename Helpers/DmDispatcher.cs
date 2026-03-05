using DSharpPlus;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.Helpers
{
    public sealed class DmDispatcher(ILogger<DmDispatcher> logger, IDmQueue queue, DiscordClient client) : BackgroundService
    {
        private readonly ILogger<DmDispatcher> _logger = logger;
        private readonly IDmQueue _queue = queue;
        private readonly DiscordClient _client = client;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DM Dispatcher started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var payload) && payload is not null)
                    {
                        await ProcessPayloadAsync((DmPayload)payload, stoppingToken);
                        await Task.Delay(1500, stoppingToken); // rate limit safety
                    }
                    else
                    {
                        await Task.Delay(250, stoppingToken); // idle wait
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in DM dispatcher loop.");
                    await Task.Delay(2000, stoppingToken);
                }
            }

            _logger.LogInformation("DM Dispatcher Completed.");
        }

        private async Task ProcessPayloadAsync(DmPayload payload, CancellationToken ct)
        {
            try
            {
                var user = await _client.GetUserAsync(payload.UserId);
                if (user is null)
                {
                    _logger.LogWarning("User {UserId} not found for DM.", payload.UserId);
                    return;
                }

                await user.SendMessageAsync(payload.Message);
                _logger.LogInformation("Sent DM to {UserId}.", payload.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send DM to {UserId}.", payload.UserId);
            }
        }


    }
}
