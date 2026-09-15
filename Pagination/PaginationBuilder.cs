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
        public static IReadOnlyList<DiscordComponent> CreateNavigationButtons(int currentPage, int totalPages, ulong msgId)
        {
            return
            [
                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"configure:prev:{msgId}",
                    "◀ Previous",
                    currentPage == 0),
                new DiscordButtonComponent(
                    DiscordButtonStyle.Secondary,
                    $"configure:page:{msgId}",
                    $"Page {currentPage + 1}/{totalPages}",
                    true),
                new DiscordButtonComponent(
                    DiscordButtonStyle.Primary,
                    $"configure:next:{msgId}",
                    "Next ▶",
                    currentPage >= totalPages - 1)
            ];
        }
    }
}
