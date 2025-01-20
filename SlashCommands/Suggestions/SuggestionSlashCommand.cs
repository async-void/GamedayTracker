using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Suggestions
{
    public class SuggestionSlashCommand: ApplicationCommandModule
    {
        [SlashCommand("suggestion", "creates a suggestion")]
        public async Task CreateSuggestion(InteractionContext ctx, [Option("title", "suggestion title")] string title,
            [Option("suggestion", "the suggestion description")] string description)
        {

        }
    }
}
