using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Message
{
    public class MessageCreatedEventHandler : IEventHandler<MessageCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, MessageCreatedEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
