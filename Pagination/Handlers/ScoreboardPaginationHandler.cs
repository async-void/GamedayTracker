using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;

namespace GamedayTracker.Pagination.Handlers
{
    public class ScoreboardPaginationHandler(IDiscordEmbedService embedService) : IPaginationHandler
    {
        
        public async Task HandleNextAsync(object data, InteractionCreatedEventArgs eventArgs)
        {
            var pagination = (PaginationData)data;

            pagination.CurrentPage++;

            var msg = await embedService.CreateScoreboardPage(
                pagination.Scoreboard,
                pagination.Emoji,
                pagination.Season,
                pagination.CurrentPage);

            var buttons = PaginationBuilder.CreateNavigationButtons(pagination.CurrentPage, pagination.TotalPages, eventArgs.Interaction.Message.Id);
            msg.AddActionRowComponent(new DiscordActionRowComponent(buttons));

            await eventArgs.Interaction.CreateResponseAsync(
                 DiscordInteractionResponseType.UpdateMessage,
                 new DiscordInteractionResponseBuilder(msg));
        }

        public async Task HandlePreviousAsync(object data, InteractionCreatedEventArgs eventArgs)
        {
            var pagination = (PaginationData)data;

            pagination.CurrentPage--;

            var msg = await embedService.CreateScoreboardPage(
                pagination.Scoreboard,
                pagination.Emoji,
                pagination.Season,
                pagination.CurrentPage);

            var buttons = PaginationBuilder.CreateNavigationButtons(pagination.CurrentPage, pagination.TotalPages, eventArgs.Interaction.Message.Id);
            msg.AddActionRowComponent(new DiscordActionRowComponent(buttons));

            await eventArgs.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder(msg));
        }
    }
 }


