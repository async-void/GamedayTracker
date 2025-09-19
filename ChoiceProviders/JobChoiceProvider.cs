using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.ChoiceProviders
{
    public class JobChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Jobs =
        [
            new DiscordApplicationCommandOptionChoice("RealTimeScoresJob", 1),
            new DiscordApplicationCommandOptionChoice("DailyHeadlinesJob", 2),
            new DiscordApplicationCommandOptionChoice("DailyStandingsJob", 3),

        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Jobs);
        }
    }
}
