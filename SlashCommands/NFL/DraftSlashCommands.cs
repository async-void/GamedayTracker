using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Interfaces;

namespace GamedayTracker.SlashCommands.NFL
{
    [Command("Draft")]
    [Description("Draft group commands")]
    public class DraftSlashCommands(ITeamData teamData)
    {
        [Command("get")]
        [Description("Get supplied draft season")]
        public async Task GetDraftSeason(SlashCommandContext ctx, [SlashChoiceProvider<ConferenceChoiceProvider>] int conference)
        {
            await ctx.DeferResponseAsync();

            switch (conference)
            {
                case 0:
                    var afcOptions = teamData.BuildSelectOptionForAfc();
                    var afcDropDown = new DiscordSelectComponent("afcDropdown", "AFC Options", afcOptions.Value);

                    DiscordComponent[] components =
                    [
                        new DiscordTextDisplayComponent("# AFC Draft Results"),
                        new DiscordSeparatorComponent(true),
                        new DiscordActionRowComponent([afcDropDown])
                    ];
                    var optionEmbed = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(new DiscordContainerComponent(components, false, DiscordColor.DarkGray));

                    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(optionEmbed));

                    break;
                case 1:
                    var nfcOptions = teamData.BuildSelectOptionForNfc();
                    var nfcDropDown = new DiscordSelectComponent("nfcDropdown", "NFC Options", nfcOptions.Value);
                    components =
                    [
                        new DiscordTextDisplayComponent("# AFC Draft Results"),
                        new DiscordSeparatorComponent(true),
                        new DiscordActionRowComponent([nfcDropDown])
                    ];
                    optionEmbed = new DiscordMessageBuilder()
                        .EnableV2Components()
                        .AddContainerComponent(new DiscordContainerComponent(components, false, DiscordColor.DarkGray));

                    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(optionEmbed));

                    break;
                case 2:
                    var drafts = await teamData.GetDraftResultsAsync("", 2026);
                    break;
            }
        }
    }
}
