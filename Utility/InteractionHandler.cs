using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace GamedayTracker.Utility
{
    public class InteractionHandler: IEventHandler<InteractionCreatedEventArgs>
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
                    switch (eventArgs.Interaction.Data.Values[0])
                        {
                            case "ldbServer":
                                {
                                    await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Server Option Selected!"));
                                    break;
                                }
                            case "ldbGlobal":
                            {
                                await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Global Option Selected!"));
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
