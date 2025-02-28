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
    public class NfcChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> NfcChoices =
        [
            new DiscordApplicationCommandOptionChoice("Dallas Cowboys", 0),
            new DiscordApplicationCommandOptionChoice("New York Giants", 1),
            new DiscordApplicationCommandOptionChoice("Philadelphia Eagles", 2),
            new DiscordApplicationCommandOptionChoice("Washington Commanders", 3),
            new DiscordApplicationCommandOptionChoice("Chicago Bears", 4),
            new DiscordApplicationCommandOptionChoice("Detroit Lions", 5),
            new DiscordApplicationCommandOptionChoice("Green Bay Packers", 6),
            new DiscordApplicationCommandOptionChoice("Minnesota Vikings", 7),
            new DiscordApplicationCommandOptionChoice("Atlanta Falcons", 8),
            new DiscordApplicationCommandOptionChoice("Carolina Panthers", 9),
            new DiscordApplicationCommandOptionChoice("New Orleans Saints", 10),
            new DiscordApplicationCommandOptionChoice("Tampa Bay Buccaneers", 11),
            new DiscordApplicationCommandOptionChoice("Arizona Cardinals", 12),
            new DiscordApplicationCommandOptionChoice("Los Angeles Rams", 13),
            new DiscordApplicationCommandOptionChoice("San Francisco 49ers", 14),
            new DiscordApplicationCommandOptionChoice("Seattle Seahawks", 15),
        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(NfcChoices);
        }
    }
}
