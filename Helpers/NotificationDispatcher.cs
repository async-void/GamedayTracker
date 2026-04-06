using DSharpPlus;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GamedayTracker.Helpers
{
    public sealed class NotificationDispatcher(ILogger<NotificationDispatcher> logger, DiscordClient client, IDmQueue dispatcherQueue) : BackgroundService
    {
        private readonly ILogger<NotificationDispatcher> _logger = logger;
        private readonly DiscordClient _client = client;
        private readonly IDmQueue _dispatcherQueue = dispatcherQueue;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           _logger.LogInformation("Notification Dispatcher started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_dispatcherQueue.TryDequeue(out var payload) && payload is not null)
                    {
                        await ProcessPayloadAsync((NotificationPayload)payload, stoppingToken);
                        await Task.Delay(1500, stoppingToken); // rate limit safety
                    }
                    else
                    {
                        await Task.Delay(250, stoppingToken); // idle wait
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error: {error}", ex.Message);
                    await Task.Delay(2000, stoppingToken);
                }
            }
            _logger.LogInformation("Notification Dispatcher Completed.");
        }

        private async Task ProcessPayloadAsync(NotificationPayload payload, CancellationToken ct)
        {
            try
            {
                var guild = await _client.GetGuildAsync(payload.GuildId);
                if (guild is null)
                {
                    _logger.LogWarning("Guild not found for {guild}.", payload.GuildId);
                    return;
                }
                var notificationChannel = guild.GetDefaultChannel();

                if (notificationChannel is not null)
                {
                    await notificationChannel.SendMessageAsync(payload.Message);
                    _logger.LogInformation("Sent message to {GuildId}.", payload.GuildId);
                }
                else
                    _logger.LogInformation("Notification Channel not found: GuildId: {GuildId} | Guild Name: {guildName}.", payload.GuildId, guild.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to {GuildId}.", payload.GuildId);
            }
        }
    }
}
