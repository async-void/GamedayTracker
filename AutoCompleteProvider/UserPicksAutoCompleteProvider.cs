using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace GamedayTracker.AutoCompleteProvider
{
    public class UserPicksAutoCompleteProvider : IAutoCompleteProvider
    {
        public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            return new ValueTask<IEnumerable<DiscordAutoCompleteChoice>>();
        }
    }
}
