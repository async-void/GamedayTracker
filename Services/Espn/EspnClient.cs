using GamedayTracker.Models.NFL;
using System.Text.Json;

namespace GamedayTracker.Services.Espn
{
    public sealed class EspnClient(HttpClient httpClient, EspnOptions options) : IEspnClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly EspnOptions _options = options;
        public async Task<Season> GetSeasonAsync(CancellationToken ct = default)
        {
            var url = NflEndpoints.Season(_options.CoreBaseUrl);
            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var season = JsonSerializer.Deserialize<Season>(json);
            return season ?? new Season();
        }

        #region TEAM ROSTER
        public async Task<List<Athlete>> GetTeamRosterAsync(string teamId, CancellationToken ct = default)
        {
            var url = NflEndpoints.Roster(_options.SiteBaseUrl, teamId);
            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var roster = JsonSerializer.Deserialize<List<Athlete>>(json);
            return roster ?? [];
        }
        #endregion

        #region GET TEAM
        public async Task<NFLTeam> GetTeam(string teamId, CancellationToken ct = default)
        {
            var url = NflEndpoints.Team(_options.CoreBaseUrl, teamId);
            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var team = JsonSerializer.Deserialize<NFLTeam>(json);
            return team ?? new NFLTeam();
        }
        #endregion

        #region GET STANDINGS
        public async Task<NflStandings> GetStandingsAsync(string? season = null, CancellationToken ct = default)
        {
            var url = NflEndpoints.Standings(_options.CoreBaseUrl);
            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var standings = JsonSerializer.Deserialize<NflStandings>(json)
                ?? throw new InvalidOperationException("Failed to deserialize standings");
            return standings;
        }

        #endregion

        #region GET SCOREBOARD
        public async Task<NFLScoreboard> GetScoreboardAsync(string? season,string? week,string? seasonType,CancellationToken ct = default)
        {
            var noParams =
                string.IsNullOrWhiteSpace(season) &&
                string.IsNullOrWhiteSpace(week) &&
                string.IsNullOrWhiteSpace(seasonType);

            var url = noParams
                ? NflEndpoints.Scoreboard(_options.SiteBaseUrl)
                : NflEndpoints.Scoreboard(_options.SiteBaseUrl, season!, week!, seasonType!);

            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Scoreboard JSON was empty.");

            return JsonSerializer.Deserialize<NFLScoreboard>(json)
                ?? throw new InvalidOperationException("Scoreboard JSON could not be deserialized.");
        }

        #endregion
    }
}
