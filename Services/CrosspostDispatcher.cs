using DSharpPlus;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace GamedayTracker.Services
{
    public sealed class CrosspostDispatcher(DiscordClient client, ILogger<CrosspostDispatcher> logger)
    {
        private readonly DiscordClient _client = client;
        private readonly ILogger<CrosspostDispatcher> _logger = logger;
        private readonly Channel<Func<Task>> _queue = Channel.CreateUnbounded<Func<Task>>();
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(1);

        public void Start()
        {
            _ = Task.Run(async () =>
            {
                while(true)
                {
                    await _queue.Reader.WaitToReadAsync();
                    while (_queue.Reader.TryRead(out var workItem))
                    {
                        try
                        {
                            await workItem();
                        }
                        catch (Exception ex)
                        {
                            // Log the exception or handle it as needed
                            _logger.LogInformation("Error executing work item: {Message}", ex.Message);
                        }
                        await Task.Delay(_delay);
                    }

                    await Task.Delay(_delay);
                }
            });
        }

        public void Enqueue(Func<Task> workItem)
        {
            if (!_queue.Writer.TryWrite(workItem))
            {
                _logger.LogInformation("Failed to enqueue work item.");
            }
        } 
    }
}
