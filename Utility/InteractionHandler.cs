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

                                    //results found...notify the user!
                                    foreach (var draftEntity in draftResult.Value)
                                    {
                                        msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                                    }
                                    var draftMessage = new DiscordEmbedBuilder()
                                            .WithTitle($"2024 Draft Results")
                                            .WithDescription(msgBuilder.ToString());
                                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(draftMessage));
                                    break;
                                }
                            case "nfcDropdown":
                            {
                                var tName = eventArgs.Interaction.Data.Values[0];
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"NFC Option Selected: {tName}"));
                                //Build the response with team name and client ctx
                                var draftResult = await teamData.GetDraftResultForTeamAsync(2020, tName);
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
