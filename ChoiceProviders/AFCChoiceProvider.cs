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
    public class AfcChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> AfcChoices =
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
        ];
            
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(AfcChoices);
        }
    }
}
