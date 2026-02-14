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
    public class SeasonTypeChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> SeasonTypes =
        [
            new DiscordApplicationCommandOptionChoice("Pre Season", 1),
            new DiscordApplicationCommandOptionChoice("Regular Season", 2),
            new DiscordApplicationCommandOptionChoice("Post Season", 3),
        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(SeasonTypes);
        }
    }
}
