﻿using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace GamedayTracker.SlashCommands.Suggestions
{
    public class SuggestionSlashCommand
    {
        [Command("suggestion")]
        [Description("creates a suggestion")]
        public async Task CreateSuggestion(SlashCommandContext ctx, [Parameter("title")] string title,
            [Parameter("suggestion")] string description)
        {
            await ctx.DeferResponseAsync();
            //1. check if suggestion already exists.
            //2. if not, create a new suggestion.
            //3. create a new suggestion thread if it doesn't exist.
            //4. create a new suggestion message in the thread.
            //5. send a confirmation message to the user.
        }
    }
}
