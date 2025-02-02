using System.ComponentModel;
using DSharpPlus.Commands;

using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Suggestions
{
    public class SuggestionSlashCommand
    {
        [DSharpPlus.Commands.Command("suggestion")]
        [Description("creates a suggestion")]
        public async Task CreateSuggestion(CommandContext ctx, [Option("title", "suggestion title")] string title,
            [Option("suggestion", "the suggestion description")] string description)
        {
            await ctx.DeferResponseAsync();
        }
    }
}
