using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.ChoiceProviders
{
    public class StatsChoiceProvider : IChoiceProvider
    {
        private static IEnumerable<DiscordApplicationCommandOptionChoice> choices =
        [
            new DiscordApplicationCommandOptionChoice("Russing", 0),
            new DiscordApplicationCommandOptionChoice("Passing", 1),
            new DiscordApplicationCommandOptionChoice("Receiving", 2),
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
           return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(choices);
        }
    }
}
