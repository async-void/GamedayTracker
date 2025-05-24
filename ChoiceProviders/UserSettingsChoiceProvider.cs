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
    public class UserSettingsChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> SettingsChoices =
        [
            new DiscordApplicationCommandOptionChoice("Favorite Team", 0),
            new DiscordApplicationCommandOptionChoice("Enable Updates", 1),
            new DiscordApplicationCommandOptionChoice("Enable Notifications", 2),

        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(SettingsChoices);
        }
    }
}
