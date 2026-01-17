using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class LeaderDetail
    {
        [JsonPropertyName("displayValue")]
        public string DisplayValue { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("athlete")]
        public Athlete Athlete { get; set; }

        [JsonPropertyName("team")]
        public TeamReference Team { get; set; }
    }
}
