﻿using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.Utility
{
    public class InteractionHandler(ITeamData teamData): IEventHandler<InteractionCreatedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, InteractionCreatedEventArgs eventArgs)
        {
            switch (eventArgs.Interaction.Type)
            {
                case DiscordInteractionType.ApplicationCommand:
                {
                    break;
                }
                case DiscordInteractionType.Component:
                {
                    switch (eventArgs.Interaction.Data.CustomId)
                        {
                            #region AFC DROPDOWN
                            case "afcDropdown":
                            {
                                
                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2025, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }

                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"**2025** Draft Results for **{tName}**"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString()),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem("https://i.imgur.com/i6yCh8q.png"))
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(container);

                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
                                break;
                            }
                            #endregion

                            #region NFC DROPDOWN
                            case "nfcDropdown"://TODO: fix V2 component message
                            {
                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2025, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }
                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"**2025** Draft Results for **{tName}**"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString()),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem("https://i.imgur.com/i6yCh8q.png"))
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message)); 
                                break;
                            }
                            #endregion

                            #region SCOREBOARD
                            case "scoreboardHelpBtn": //TODO: here I would like to abstract this code to a method call to build the embed. not sure if it will make a difference in readability?
                            {
                                DiscordComponent[] buttons =
                                [
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")
                                ];

                                DiscordComponent[] components =
                                [
                                new DiscordTextDisplayComponent("Scoreboard Help Section"),
                                new DiscordSeparatorComponent(true),
                                new DiscordTextDisplayComponent("1. Select the Season\r2.Select the Week"),
                                new DiscordSeparatorComponent(true),
                                new DiscordTextDisplayComponent("this slash command will fetch ``All`` the game totals for the selected ``Season`` and ``Week``"),
                                new DiscordSeparatorComponent(true),
                                new DiscordActionRowComponent(buttons)
                                ];

                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
                                var scoreboardMessage = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);

                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(scoreboardMessage));
                                break;
                            }
                            #endregion

                            #region BUTTONS
                            case "donateId":
                                DiscordComponent[] bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "Donate is in development, the devs are hard at work implementing this feature!")
                                ];
                                var cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.LightGray);
                                var bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(
                                    DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            case "art1Id": 
                                bComponent =
                                [
                                    new DiscordTextDisplayComponent(
                                        "Read More is in development, the devs are hard at work implementing this feature!")
                                ];
                                 cContainer =
                                    new DiscordContainerComponent(bComponent, false, DiscordColor.LightGray);
                                 bMsg = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(cContainer);
                                await eventArgs.Interaction.CreateResponseAsync(
                                    DiscordInteractionResponseType.UpdateMessage,
                                    new DiscordInteractionResponseBuilder(bMsg));
                                break;
                            #endregion

                            default:
                                {
                                    break;
                                }
                        }
                    break;
                }
                case DiscordInteractionType.Ping:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ping is in development, the devs are hard at work implementing this feature!"));
                    break;
                case DiscordInteractionType.AutoComplete:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Auto Complete is in development, the devs are hard at work implementing this feature!"));
                    break;
                case DiscordInteractionType.ModalSubmit:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Modal Submit is in development, the devs are hard at work implementing this feature!"));
                    break;
                default:
                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Unknown command!"));
                    return;
            }
        }
    }
}
