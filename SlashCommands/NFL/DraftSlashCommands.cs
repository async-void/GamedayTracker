using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands.Localization;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.NFL
{
    [Command("Draft")]
    [Description("Draft group commands")]
    public class DraftSlashCommands(ITeamData _teamData)
    {
        //TODO: convert int to TeamName
        [Command("get")]
        [Description("Get supplied draft season")]
        public async Task GetDraftSeason(CommandContext ctx, [SlashChoiceProvider<ConferenceChoiceProvider>] int conference)
        {
            await ctx.DeferResponseAsync();

            switch (conference)
            {
                case 0:
                    var afcOptions = _teamData.BuildSelectOptionForAfc();
                    var afcDropDown = new DiscordSelectComponent("afcDropdown", "AFC Options", afcOptions.Value);
                    var optionEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle("AFC Options"))
                        .AddComponents(afcDropDown);
                    await ctx.EditResponseAsync(optionEmbed);
                    
                    break;
                case 1:
                    var nfcOptions = _teamData.BuildSelectOptionForNfc();
                    var nfcDropDown = new DiscordSelectComponent("nfcDropdown", "NFC Options", nfcOptions.Value); 
                    optionEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle("NFC Options"))
                        .AddComponents(nfcDropDown);
                    await ctx.EditResponseAsync(optionEmbed);
                    break;
            }
               

                //no results found..... notify the user!
                //if (draft.Count == 0)
                //{
                //    await ctx.EditResponseAsync($"no results found for **test** in season **{season}**");
                //    return;
                //}

                //var msgBuilder = new StringBuilder();

                ////results found...notify the user!
                //foreach (var draftEntity in draft)
                //{
                //    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                //}
                //var draftMessage = new DiscordMessageBuilder()
                //    .AddEmbed(new DiscordEmbedBuilder()
                //        .WithTitle($"test {season} Draft Results")
                //        .WithDescription(msgBuilder.ToString()));

                //await ctx.EditResponseAsync(draftMessage);
            
        }
    }
}
