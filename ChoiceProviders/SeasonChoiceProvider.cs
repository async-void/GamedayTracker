using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;

namespace GamedayTracker.ChoiceProviders
{
    public class SeasonChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Seasons =
        [
            new DiscordApplicationCommandOptionChoice("2020", 2020),
            new DiscordApplicationCommandOptionChoice("2021", 2021),
            new DiscordApplicationCommandOptionChoice("2022", 2022),
            new DiscordApplicationCommandOptionChoice("2023", 2023),
            new DiscordApplicationCommandOptionChoice("2024", 2024),
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Seasons);
        }
    }
}
