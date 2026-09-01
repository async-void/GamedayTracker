using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Interfaces;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.Settings
{
    [Command("settings")]
    [Description("Settings related commands")]
    public class SettingsSlashCommands(IJsonDataService jsonDataService)
    {
        private readonly IJsonDataService _jsonDataService = jsonDataService;


        #region TOGGLE NOTIFICATIONS
        [Command("toggleNotifications")]
        [Description("toggle system wide notifications")]
        [RequirePermissions(permissions: DiscordPermission.Administrator)]
        public async Task ToggleServerNotifications(SlashCommandContext ctx, [SlashChoiceProvider<ToggleChoiceProvider>] int choice)
        {
            await ctx.DeferResponseAsync();
            var guildId = ctx.Guild!.Id;
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
        #endregion

        #region CONFIGURE DAILY STANDINGS / REALTIME SCORES / DAILY NEWS
        [Command("configure")]
        [Description("configure Daily Headlines | Daily News | Daily Standings")]
        [RequirePermissions(permissions: DiscordPermission.ManageGuild)]
        public async ValueTask Configure(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(ephemeral: true);

            //build the container with the buttons to configure different sections

            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent("## Configure Daily Settings"),
                new DiscordSectionComponent(new DiscordTextDisplayComponent("Configure Daily Standings"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "configure:standings", "Standings")),
                new DiscordSectionComponent(new DiscordTextDisplayComponent("Configure Daily News"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "configure:news", "News")),
                new DiscordSectionComponent(new DiscordTextDisplayComponent("Configure Live Scores"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "configure:scores", "Scores")),
            ];

            var container = new DiscordContainerComponent(comps, false, DiscordColor.Gray);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            await ctx.RespondAsync(msg);

        }
        #endregion
    }
}
