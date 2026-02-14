using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using GamedayTracker.Models.NFL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.ChoiceProviders
{
    public class GameDayChoiceProvider : IChoiceProvider
    {
        private static readonly IEnumerable<DiscordApplicationCommandOptionChoice> Days =
        [
            new DiscordApplicationCommandOptionChoice("Monday", 1),
            new DiscordApplicationCommandOptionChoice("Thursday", 2),
            new DiscordApplicationCommandOptionChoice("Sunday", 3)
        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Days);
        }
    }
}
