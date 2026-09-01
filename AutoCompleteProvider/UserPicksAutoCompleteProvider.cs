using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.AutoCompleteProvider
{
    public class UserPicksAutoCompleteProvider(IGameData gameService) : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            var scoreboard = await gameService.GetNFLScoresAsync();
            var result = new List<DiscordAutoCompleteChoice>();

            var eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            var sundayGames = scoreboard.Events?
            .Where(e =>
            {
                var local = e.Date;
                return local.Value.DayOfWeek == DayOfWeek.Sunday;
            })
            .ToList() ?? [];

            if (sundayGames.Count == 0)
            {
                result?.Add(new("No choices found!", null));
                return result!;
            }
            foreach (var game in sundayGames)
            {
                if (result.Count >= 25) return result;
                
                var awayTeamName = game.Competitions[0].Competitors[0].Team.Name;
                var homeTeamName = game.Competitions[0].Competitors[1].Team.Name;
                var choiceOne = $"{awayTeamName} beats {homeTeamName}";
                var choiceTwo = $"{homeTeamName} beats {awayTeamName}";
                result.Add(new DiscordAutoCompleteChoice(choiceOne, choiceOne));
                result.Add(new DiscordAutoCompleteChoice(choiceTwo, choiceTwo));
            }

            if (result.Count == 0 || result is null) result?.Add(new("No choices found!", null));
            return result!;
        }
    }
}
