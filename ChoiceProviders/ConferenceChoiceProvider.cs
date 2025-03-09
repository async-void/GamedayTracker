using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;

namespace GamedayTracker.ChoiceProviders
{
    public class ConferenceChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Conferences =
        [
            new DiscordApplicationCommandOptionChoice("AFC", 0),
            new DiscordApplicationCommandOptionChoice("NFC", 1),
            new DiscordApplicationCommandOptionChoice("All", 2)
        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Conferences);
        }
    }
}
