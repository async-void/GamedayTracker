using System.ComponentModel;
using DSharpPlus.Commands;

namespace GamedayTracker.SlashCommands.Suggestions
{
    public class SuggestionSlashCommand
    {
        [DSharpPlus.Commands.Command("suggestion")]
        [Description("creates a suggestion")]
        public async Task CreateSuggestion(CommandContext ctx, [Parameter("title")] string title,
            [Parameter("suggestion")] string description)
        {
            await ctx.DeferResponseAsync();
        }
    }
}
