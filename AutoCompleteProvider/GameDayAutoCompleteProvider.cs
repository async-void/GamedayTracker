using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;

namespace GamedayTracker.AutoCompleteProvider
{
    public class GameDayAutoCompleteProvider : IAutoCompleteProvider
    {
        private readonly IGameData _gameData;

        public GameDayAutoCompleteProvider(IGameData gameData)
        {
            _gameData = gameData;
        }

        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            var games = await _gameData.GetNFLScoresAsync();

            if (games?.Events == null || games.Events.Count == 0)
            {
                return
                [
                    new DiscordAutoCompleteChoice("No Scheduled Games for this Week!", "none")
                ];
            }

            var days = games.Events
                .Select(g => g.Date.DayOfWeek)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return days.Select(d =>
                new DiscordAutoCompleteChoice(
                    name: d.ToString(),
                    value: d.ToString()
                )
            );


        }
    }
}
