using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;

namespace GamedayTracker.ChoiceProviders
{
    public class WeekChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Weeks =
        [
            new DiscordApplicationCommandOptionChoice("1", 1),
            new DiscordApplicationCommandOptionChoice("2", 2),
            new DiscordApplicationCommandOptionChoice("3", 3),
            new DiscordApplicationCommandOptionChoice("4", 4),
            new DiscordApplicationCommandOptionChoice("5", 5),
            new DiscordApplicationCommandOptionChoice("6", 6),
            new DiscordApplicationCommandOptionChoice("7", 7),
            new DiscordApplicationCommandOptionChoice("8", 8),
            new DiscordApplicationCommandOptionChoice("9", 9),
            new DiscordApplicationCommandOptionChoice("10", 10),
            new DiscordApplicationCommandOptionChoice("11", 11),
            new DiscordApplicationCommandOptionChoice("12", 12),
            new DiscordApplicationCommandOptionChoice("13", 13),
            new DiscordApplicationCommandOptionChoice("14", 14),
            new DiscordApplicationCommandOptionChoice("15", 15),
            new DiscordApplicationCommandOptionChoice("16", 16),
            new DiscordApplicationCommandOptionChoice("17", 17),
            new DiscordApplicationCommandOptionChoice("18", 18),
            new DiscordApplicationCommandOptionChoice("Wild Card Playoff", 19),
            new DiscordApplicationCommandOptionChoice("Divisional Playoff", 20),
            new DiscordApplicationCommandOptionChoice("Conference Championship", 21),
            new DiscordApplicationCommandOptionChoice("Super Bowl", 22),
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Weeks);
        }
    }
}
