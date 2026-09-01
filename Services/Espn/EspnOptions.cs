namespace GamedayTracker.Services.Espn
{
    public sealed class EspnOptions
    {
        public string CoreBaseUrl { get; init; } = "https://sports.core.api.espn.com/v2/sports/football/leagues/nfl/";
        public string SiteBaseUrl { get; init; } = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/";

    }
}
