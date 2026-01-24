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
        Task<DiscordEmbed> CreateScoresEmbed(NFLScoreboard data);
        Task<List<DiscordEmbed>> CreateStandingsEmbedsByConferenceAsync();
        Task<DiscordEmbed> CreateStandingsEmbedAsync(string? conference = null);
        Task<DiscordMessageBuilder> CreateTeamStatsEmbed(NflTeamStatisticsResponse teamStats, NFLSeasonType seasonType, int seasonYear, string teamAbbr);
        Task<DiscordMessageBuilder> CreateTeamStatsPage(NflTeamStatisticsResponse teamStats, string emoji, NFLSeasonType seasonType, int seasonYear, int pageIndex);
    }
}
