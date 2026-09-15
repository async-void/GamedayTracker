using GamedayTracker.Models.News;
using GamedayTracker.Models.NFL;

namespace GamedayTracker.Services.Espn
{
    public interface IEspnClient
    {
        Task<Season> GetSeasonAsync(CancellationToken ct = default);
        Task<List<Athlete>> GetTeamRosterAsync(string teamId, CancellationToken ct = default);
        Task<NFLTeam> GetTeam(string teamId, CancellationToken ct = default);
        Task<NflStandings> GetStandingsAsync(string? season = null, CancellationToken ct = default);
        Task<NFLScoreboard> GetScoreboardAsync(string? season, string? week, string? seasonType, CancellationToken ct = default);
        Task<List<NewsArticle>> GetNewsAsync(CancellationToken ct = default);
        Task<NFLScoreboard> GetTeamScheduleAsync(string teamId, CancellationToken ct = default);
    }
}
