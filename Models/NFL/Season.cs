using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Season
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("type")]
        public SeasonType Type { get; set; }
    }
}
