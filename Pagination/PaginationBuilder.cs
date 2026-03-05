using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Pagination
{
    public static class PaginationBuilder
    {
        public static IReadOnlyList<DiscordComponent> CreateNavigationButtons(int currentPage, int totalPages)
        {
            return
            [
                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"prev",
                    "◀ Previous",
                    currentPage == 0),
                new DiscordButtonComponent(
                    DiscordButtonStyle.Secondary,
                    $"page",
                    $"Page {currentPage + 1}/{totalPages}",
                    true),
                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"next",
                    "Next ▶",
                    currentPage >= totalPages - 1)
            ];
        }
    }
}
