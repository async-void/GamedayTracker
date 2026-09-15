using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Cache;
using GamedayTracker.Interfaces;
using GamedayTracker.Pagination;
using GamedayTracker.Services;
using Microsoft.Extensions.DependencyInjection;

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
                case "next":
                    await HandleConfigureInteractionAsync(sender, args);
                    break;
                case "prev": 
                    await HandleConfigureInteractionAsync(sender, args);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleConfigureInteractionAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
        {
            var compId = args.Interaction.Data.CustomId;
            var user = args.Interaction.User;
            if (compId.StartsWith("configure:"))
            {
                var parts = compId.Split(":");
                var msgId = ulong.Parse(parts[2]);
                var pageData = ScoreboardPaginationCache.Get(msgId);
                var embedService = sender.ServiceProvider.GetRequiredService<IDiscordEmbedService>();

                if (pageData is null)
                {
                    await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("This pagination session has expired. Please start a new one.")
                            .AsEphemeral(true));
                    return;
                }

                switch (parts[1])
                {
                    case "next":
                        pageData.CurrentPage++;
                        break;
                    case "prev":
                        pageData.CurrentPage--;
                        break;
                }
                // clamp
                if (pageData.CurrentPage < 0)
                    pageData.CurrentPage = 0;

                if (pageData.CurrentPage > pageData.TotalPages)
                    pageData.CurrentPage = pageData.TotalPages;

                var newPage = await embedService.CreateScoreboardPage(
                    pageData.Scoreboard,
                    pageData.Emoji,
                    pageData.Season,
                    pageData.CurrentPage
                );

                var buttons = PaginationBuilder.CreateNavigationButtons(
                    pageData.CurrentPage,
                    pageData.TotalPages,
                    args.Interaction.Message.Id
                );

                newPage.AddActionRowComponent(new DiscordActionRowComponent(buttons));

                await args.Interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder(newPage)
                );

                return;

            }
            else if (compId.StartsWith("bet"))//HANDLE THE BETS HERE
            {
                var test = "";
                var gameId = compId.Split(":")[2];
                //BettingService.HandleBettingInteraction(args.Interaction, user, gameId);
                return;
            }
            var compType = args.Interaction.Data.ComponentType;
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
