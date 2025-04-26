using System.Text;
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
                            case "afcDropdown":
                            {
                                
                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2024, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }

                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"Draft Results for **{tName}**"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString())
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                .EnableV2Components()
                                .AddContainerComponent(container);

                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
                                break;
                            }
                            case "nfcDropdown"://TODO: fix V2 component message
                            {
                                var tName = eventArgs.Interaction.Data.Values[0];
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2024, tName);
                                var msgBuilder = new StringBuilder();

                                foreach (var draftEntity in draftResult.Value)
                                {
                                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                }
                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent($"Draft Results for **{tName}**"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordTextDisplayComponent(msgBuilder.ToString())
                                ];
                                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);

                                var message = new DiscordInteractionResponseBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message)); 
                                break;
                            }
                            case "scoreboardHelpBtn": //TODO: here I would like to abstract this code to a method call to build the embed. not sure if it will make a difference in readability?
                            {
                                DiscordComponent[] buttons =
                                [
                                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "donateId", "Donate", false),
                                    new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId1", "Donate", false),
                                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId2", "Donate", false),
                                ];

                                DiscordComponent[] components =
                                [
                                new DiscordTextDisplayComponent("Scoreboard is in development, the devs are hard at work implementing this feature!"),
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
