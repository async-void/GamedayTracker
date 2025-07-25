using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace GamedayTracker.SlashCommands.Suggestions
{
    public class SuggestionSlashCommand
    {
        [Command("suggestion")]
        [Description("creates a suggestion")]
        public async ValueTask CreateSuggestion(SlashCommandContext ctx, [Parameter("message")] DiscordMessage message,
            [Parameter("thread-name")] string name)
        {
            await ctx.DeferResponseAsync();
            //1. check if suggestion already exists.
            //2. if not, create a new suggestion.
            //3. create a new suggestion thread if it doesn't exist.
            //4. create a new suggestion message in the thread.
            //5. send a confirmation message to the user.
            var chnl = await ctx.Guild!.GetChannelAsync(1395298980111581215);
            await chnl.CreateThreadAsync(message, name, DiscordAutoArchiveDuration.ThreeDays, "support");
        }
    }
}
