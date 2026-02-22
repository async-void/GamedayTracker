using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL;

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
        Task<DiscordMessageBuilder> CreateScoreboardPage(NFLScoreboard scores, string emoji, NFLSeasonType seasonType, int seasonYear, int pageIndex);
        Task<DiscordMessageBuilder> BuildBettingEmbed(string data, string amount);
        Task<DiscordContainerComponent> BuildBettingResultEmbed(Bet bet);
        Task<DiscordContainerComponent> BuildErrorContainer(DiscordClient client, string errorMessage, ulong guildId, DiscordColor color);
    }
}
