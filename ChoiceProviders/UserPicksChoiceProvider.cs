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
    public class UserPicksChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Choices =
        [
            new DiscordApplicationCommandOptionChoice("Buffalo Bills", 0),
            new DiscordApplicationCommandOptionChoice("Miami Dolphins", 1),
            new DiscordApplicationCommandOptionChoice("New England Patriots", 2),
            new DiscordApplicationCommandOptionChoice("New York Jets", 3),
            new DiscordApplicationCommandOptionChoice("Baltimore Ravens", 4),
            new DiscordApplicationCommandOptionChoice("Cincinnati Bengals", 5),
            new DiscordApplicationCommandOptionChoice("Cleveland Browns", 6),
            new DiscordApplicationCommandOptionChoice("Pittsburgh Steelers", 7),
            new DiscordApplicationCommandOptionChoice("Houston Texans", 8),
            new DiscordApplicationCommandOptionChoice("Indianapolis Colts", 9),
            new DiscordApplicationCommandOptionChoice("Jacksonville Jaguars", 10),
            new DiscordApplicationCommandOptionChoice("Tennessee Titans", 11),
            new DiscordApplicationCommandOptionChoice("Denver Broncos", 12),
            new DiscordApplicationCommandOptionChoice("Kansas City Chiefs", 13),
            new DiscordApplicationCommandOptionChoice("Las Vegas Raiders", 14),
            new DiscordApplicationCommandOptionChoice("Los Angeles Chargers", 15),
            new DiscordApplicationCommandOptionChoice("Dallas Cowboys", 16),
            new DiscordApplicationCommandOptionChoice("New York Giants", 17),
            new DiscordApplicationCommandOptionChoice("Philadelphia Eagles",18),
            new DiscordApplicationCommandOptionChoice("Washington Commanders", 19),
            new DiscordApplicationCommandOptionChoice("Chicago Bears", 20),
            new DiscordApplicationCommandOptionChoice("Detroit Lions", 21),
            new DiscordApplicationCommandOptionChoice("Green Bay Packers", 22),
            new DiscordApplicationCommandOptionChoice("Minnesota Vikings", 23),
            new DiscordApplicationCommandOptionChoice("Atlanta Falcons", 24),
            new DiscordApplicationCommandOptionChoice("Carolina Panthers", 25),
            new DiscordApplicationCommandOptionChoice("New Orleans Saints", 26),
            new DiscordApplicationCommandOptionChoice("Tampa Bay Buccaneers", 27),
            new DiscordApplicationCommandOptionChoice("Arizona Cardinals", 28),
            new DiscordApplicationCommandOptionChoice("Los Angeles Rams", 29),
            new DiscordApplicationCommandOptionChoice("San Francisco 49ers", 30),
            new DiscordApplicationCommandOptionChoice("Seattle Seahawks", 31),

        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Choices);
        }
    }
}
