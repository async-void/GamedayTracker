using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class NflTeamStatisticsResponse
    {
        [JsonPropertyName("splits")]
        public Splits Splits { get; set; }

        [JsonPropertyName("team")]
        public TeamReference Team { get; set; }
    }
}
