using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Handlers.Interactions
{
    public sealed class ConfigureComponentInteractionHandler : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
        {
            var compType = args.Interaction.Type;
            if (compType != DiscordInteractionType.Component) return;
           
            var id = args.Interaction.Data.CustomId.Split(":")[1];
            switch (id)
            {
                case "standings":
                    await HandleConfigureInteractionAsync(sender, args);
                    break;
                case "news":
                    await HandleConfigureInteractionAsync(sender, args);
                    break;
                case "scores":
                    await HandleConfigureInteractionAsync(sender, args);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleConfigureInteractionAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
        {
            var compId = args.Interaction.Data.CustomId;
            var compType = args.Interaction.Data.ComponentType;
            var user = args.Interaction.User;
            var guild = args.Guild;
            // Handle the interaction based on the component ID and type
            // For example, you can send a response back to the user
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent($"You clicked on a {compType} with ID: {compId} in guild: {guild?.Name} by user: {user.Username}")
                    .AsEphemeral(true));
        }
    }
}
