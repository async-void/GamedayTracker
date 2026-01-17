using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class NFLScoreboard
    {
        [JsonPropertyName("leagues")]
        public List<League> Leagues { get; set; }

        [JsonPropertyName("season")]
        public SeasonInfo Season { get; set; }

        [JsonPropertyName("week")]
        public WeekInfo Week { get; set; }

        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }
    }
}
