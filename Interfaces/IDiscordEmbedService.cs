using DSharpPlus.Entities;
using GamedayTracker.Models.NFL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDiscordEmbedService
    {
        DiscordEmbed CreateLiveGamesEmbed(List<Event> liveGames);
        DiscordEmbed CreateScoresEmbed(NFLScoreboard data);
        Task<List<DiscordEmbed>> CreateStandingsEmbedsByConferenceAsync();
        Task<DiscordEmbed> CreateStandingsEmbedAsync(string? conference = null);
    }
}
