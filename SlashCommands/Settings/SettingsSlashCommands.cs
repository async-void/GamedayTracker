using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using GamedayTracker.Attributes;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Interfaces;

namespace GamedayTracker.SlashCommands.Settings
{
    [SlashCommandGroup("settings", "Settings related commands")]
    public class SettingsSlashCommands(IJsonDataService jsonDataService)
    {
        private readonly IJsonDataService _jsonDataService = jsonDataService;

        [SlashCommand("toggle-notifications", "turn on/off system wide notifications")]
        public async Task ToggleServerNotifications(SlashCommandContext ctx, [SlashChoiceProvider<ToggleChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            var guildId = ctx.Guild!.Id.ToString();
            var guildResult = await _jsonDataService.GetGuildFromJsonAsync(guildId);

            if (guildResult.IsOk)
            {
                var guild = guildResult.Value;
                if (guild is { } g)
                {
                    g.ReceiveSystemMessages = choice == 0 ? false : true;
                    var updateResult = await _jsonDataService.UpdateGuildDataAsync(g);
                    if (updateResult.IsOk)
                    {
                        var successMessage = new DiscordInteractionResponseBuilder()
                            .WithContent($"Notifications have been turned on for the guild.")
                            .AsEphemeral(true);
                        await ctx.EditResponseAsync(successMessage);
                    }
                    else
                    {
                        var errMessage = new DiscordInteractionResponseBuilder()
                            .WithContent($"something went wrong, notifications we not affected\r\n{updateResult.Error.ErrorMessage}")
                            .AsEphemeral(true);
                        await ctx.EditResponseAsync(errMessage);
                    }
                }
                else
                {
                    var msg = new DiscordInteractionResponseBuilder()
                            .WithContent($"unknown error occured, please try again later.")
                            .AsEphemeral(true);
                    await ctx.EditResponseAsync(msg);
                }
            }
            else
            {
                var notFound = new DiscordInteractionResponseBuilder()
                    .WithContent("Error retrieving guild data.")
                    .AsEphemeral(true);
                await ctx.EditResponseAsync(notFound);
            }
        }
    }
}
