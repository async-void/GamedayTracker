using GamedayTracker.Models.News;
using GamedayTracker.Models.NFL;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace GamedayTracker.Services.Espn
{
    public sealed class EspnClient(EspnOptions options) : IEspnClient
    {
        private readonly EspnOptions _options = options;

        #region GET SEASON
        public async Task<Season> GetSeasonAsync(CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.Season(_options.CoreBaseUrl);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var season = JsonSerializer.Deserialize<Season>(json);
            return season ?? new Season();
        }
        #endregion

        #region TEAM ROSTER
        public async Task<List<Athlete>> GetTeamRosterAsync(string teamId, CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.Roster(_options.RosterBaseUrl, teamId);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var roster = JsonSerializer.Deserialize<List<Athlete>>(json);
            return roster ?? [];
        }
        #endregion

        #region GET TEAM
        public async Task<NFLTeam> GetTeam(string teamId, CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.Team(_options.TeamsBaseUrl, teamId);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var team = JsonSerializer.Deserialize<TeamWrapper>(json);
            return team?.Team ?? new NFLTeam();
        }
        #endregion

        #region GET TEAMS

        public async Task<List<NFLTeam>> GetTeamsAsync(CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.Teams(_options.TeamsBaseUrl);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync(ct);
            var teamsWrapper = JsonSerializer.Deserialize<List<TeamWrapper>>(json);
            return teamsWrapper?.Select(t => t.Team).ToList() ?? [];
        }

        #endregion

        #region GET TEAM SCHEDULE
        
        public async Task<NFLScoreboard> GetTeamScheduleAsync(string teamId, CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.TeamSchedule(_options.TeamsBaseUrl, teamId);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);
            var schedule = JsonSerializer.Deserialize<NFLScoreboard>(json);
            return schedule ?? new NFLScoreboard();
        }

        #endregion

        #region GET NEWS

        public async Task<List<NewsArticle>> GetNewsAsync(CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.News(_options.NewsBaseUrl);
            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync(ct);
            var newsItems = JsonSerializer.Deserialize<NewsFeed>(json);
            return newsItems?.Articles ?? [];
        }

        #endregion

        #region GET STANDINGS
        public async Task<NflStandings> GetStandingsAsync(string? season = null, CancellationToken ct = default)
        {
            var client = CreateChromeLikeClient();
            var url = NflEndpoints.Standings(_options.StandingsBaseUrl);
            using var res = await client.GetAsync(url, ct);
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
            var client = CreateChromeLikeClient();
            var noParams =
                string.IsNullOrWhiteSpace(season) &&
                string.IsNullOrWhiteSpace(week) &&
                string.IsNullOrWhiteSpace(seasonType);

            var url = noParams
                ? NflEndpoints.Scoreboard(_options.ScoresBaseUrl)
                : NflEndpoints.Scoreboard(_options.ScoresBaseUrl, season!, week!, seasonType!);

            using var res = await client.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Scoreboard JSON was empty.");

            return JsonSerializer.Deserialize<NFLScoreboard>(json)
                ?? throw new InvalidOperationException("Scoreboard JSON could not be deserialized.");
        }

        #endregion

        #region CREATE CHROME-LIKE HTTP CLIENT
        public static HttpClient CreateChromeLikeClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression =
                    DecompressionMethods.GZip |
                    DecompressionMethods.Deflate |
                    DecompressionMethods.Brotli,

                AllowAutoRedirect = true,

                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };

            var client = new HttpClient(handler)
            {
                DefaultRequestVersion = HttpVersion.Version20,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

            // --- Chrome 124 header set (Akamai-safe) ---
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
            );

            client.DefaultRequestHeaders.Accept.ParseAdd(
                "application/json, text/plain, */*"
            );

            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(
                "en-US,en;q=0.9"
            );

            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(
                "gzip, deflate, br"
            );

            // Required by Akamai bot manager
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");

            // ESPN requires a browser referer
            client.DefaultRequestHeaders.Referrer =
                new Uri("https://www.espn.com/");

            // Chrome Client Hints (these matter!)
            client.DefaultRequestHeaders.Add("Sec-Ch-Ua",
                "\"Chromium\";v=\"124\", \"Google Chrome\";v=\"124\", \"Not:A-Brand\";v=\"99\"");
            client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            client.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");

            return client;
        }
        #endregion

    }
}
