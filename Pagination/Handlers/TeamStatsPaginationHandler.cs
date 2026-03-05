using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GamedayTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Pagination.Handlers
{
    public class TeamStatsPaginationHandler(IDiscordEmbedService embedService): IPaginationHandler
    {
        public async Task HandleNextAsync(object data, InteractionCreatedEventArgs eventArgs)
        {
            var pagination = (TeamStatsPaginationData)data;

            pagination.CurrentPage++;

            var msg = await embedService.CreateTeamStatsPage(
                pagination.TeamStats,
                pagination.Emoji,
                pagination.SeasonType,
                pagination.Season,
                pagination.CurrentPage);

            var buttons = PaginationBuilder.CreateNavigationButtons(pagination.CurrentPage, pagination.TotalPages);
            msg.AddActionRowComponent(new DiscordActionRowComponent(buttons));

            await eventArgs.Interaction.CreateResponseAsync(
                 DiscordInteractionResponseType.UpdateMessage,
                 new DiscordInteractionResponseBuilder(msg));
        }

        public async Task HandlePreviousAsync(object data, InteractionCreatedEventArgs eventArgs)
        {
            var pagination = (TeamStatsPaginationData)data;

            pagination.CurrentPage--;

            var msg = await embedService.CreateTeamStatsPage(
                pagination.TeamStats,
                pagination.Emoji,
                pagination.SeasonType,
                pagination.Season,
                pagination.CurrentPage);

            var buttons = PaginationBuilder.CreateNavigationButtons(pagination.CurrentPage, pagination.TotalPages);
            msg.AddActionRowComponent(new DiscordActionRowComponent(buttons));

            await eventArgs.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder(msg));
        }
    }
}
