using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Services.Espn;

namespace GamedayTracker.AutoCompleteProvider
{
    public class GameDayAutoCompleteProvider(IEspnClient espnClient) : IAutoCompleteProvider
    {
        private readonly IEspnClient _espnClient = espnClient;

        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            var season = await _espnClient.GetSeasonAsync();
            var games = await _espnClient.GetScoreboardAsync(season.Year.ToString(), "", "");

            if (games?.Events == null || games.Events.Count == 0)
            {
                return
                [
                    new DiscordAutoCompleteChoice("No Scheduled Games for this Week!", "none")
                ];
            }

            var days = games.Events
                .Select(g => g.Date.Value.DayOfWeek)
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
