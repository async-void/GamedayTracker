using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace GamedayTracker.AutoCompleteProvider
{
    public class DailyNumbersTypeAutoCompleteProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            IReadOnlyList<DiscordAutoCompleteChoice> types =
            [
                new DiscordAutoCompleteChoice("Straight", "straight"),
                new DiscordAutoCompleteChoice("Box", "box"),
                new DiscordAutoCompleteChoice("BosStraight", "boxstraight")
            ];

            return types;
        }
    }
}
