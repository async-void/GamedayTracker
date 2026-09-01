using GamedayTracker.Models.NFL;

namespace GamedayTracker.Services.Espn
{
    public sealed class NflEndpoints
    {
        public static string Season(string baseUrl) =>
            $"{baseUrl}season";

        public static string Roster(string baseUrl, string teamId) =>
            $"{baseUrl}teams/{teamId}/roster";

        public static string Team(string baseUrl, string teamId) =>
            $"{baseUrl}teams/{teamId}";

        public static string Standings(string baseUrl) =>
           $"{baseUrl}standings";

        public static string Scoreboard(string baseUrl, string season, string week, string seasonType)
            => $"{baseUrl}scoreboard?week={week}&seasontype={seasonType}&season={season}";

        public static string Scoreboard(string baseUrl)
            => $"{baseUrl}scoreboard";
    }
}
