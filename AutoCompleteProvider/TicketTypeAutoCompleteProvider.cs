using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.AutoCompleteProvider
{
    public class TicketTypeAutoCompleteProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            IReadOnlyList<DiscordAutoCompleteChoice> types =
            [
                new DiscordAutoCompleteChoice("Support", (long)TicketType.Support),
                new DiscordAutoCompleteChoice("Bug Report", (long)TicketType.BugReport),
                new DiscordAutoCompleteChoice("Suggestion", (long)TicketType.Suggestion)
            ];

            return types;
        }
    }
}
