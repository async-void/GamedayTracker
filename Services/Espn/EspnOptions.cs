namespace GamedayTracker.Services.Espn
{
    public sealed class EspnOptions
    {
        public string CoreBaseUrl { get; init; } = "https://sports.core.api.espn.com/v2/sports/football/leagues/nfl/";
        public string NewsBaseUrl { get; init; } = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/news";
        public string ScoresBaseUrl { get; init; } = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/";
        public string StandingsBaseUrl { get; init; } = "https://site.api.espn.com/apis/v2/sports/football/nfl/";
        public string RosterBaseUrl { get; init; } = " https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/";
        public string TeamsBaseUrl { get; init; } = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/teams/";

    }
}
