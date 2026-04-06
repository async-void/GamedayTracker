using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers
{
    public class TicketInteractionHandler : IEventHandler<InteractionCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, InteractionCreatedEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
