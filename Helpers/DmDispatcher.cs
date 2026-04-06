using DSharpPlus;
using DSharpPlus.Entities;
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
                catch (OperationCanceledException op)
                {
                    _logger.LogInformation(op, "DmDispatcher graceful shutdown called in main console.");
                    return;
                }
                catch (ObjectDisposedException od)
                {
                    // Discord client or host services were disposed first
                    _logger.LogInformation(od, "DmDispatcher stopping because dependencies were disposed.");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the DM Dispatcher loop.");
                    return;
                }
            }

            _logger.LogInformation("DM Dispatcher Completed.");
        }

        private async Task ProcessPayloadAsync(DmPayload payload, CancellationToken ct)
        {
            var user = await _client.GetUserAsync(payload.UserId); ;
            try
            {
               if (user is null)
                {
                    _logger.LogWarning("User {User} not found for DM | ID: {ID}", user.Username, payload.UserId);
                    return;
                }

                await user.SendMessageAsync(payload.Message);
                _logger.LogInformation("Sent DM to {User}.", user.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send DM to {User} | with ID: {userId}", user.Username, payload.UserId);
            }
        }


    }
}
