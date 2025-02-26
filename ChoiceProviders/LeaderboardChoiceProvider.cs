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
    public class LeaderboardChoiceProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Leaderboards =
        [
            new DiscordApplicationCommandOptionChoice("Server", 0),
            new DiscordApplicationCommandOptionChoice("Global", 1),
        ];
        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            return new ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>>(Leaderboards);
        }
    }
}
