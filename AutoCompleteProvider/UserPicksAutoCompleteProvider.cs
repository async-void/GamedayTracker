using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;

namespace GamedayTracker.AutoCompleteProvider
{
    public class UserPicksAutoCompleteProvider(IGameData gameData) : IAutoCompleteProvider
    {
        private readonly IReadOnlyList<DiscordAutoCompleteChoice> BetChoices =
        [
           
        ];

        public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            return new ValueTask<IEnumerable<DiscordAutoCompleteChoice>>(BetChoices);
        }
    }
}
