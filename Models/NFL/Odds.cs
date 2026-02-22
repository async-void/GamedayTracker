using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Odds
    {
        [JsonPropertyName("provider")]
        public OddsProvider Provider { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }

        [JsonPropertyName("overUnder")]
        public double? OverUnder { get; set; }

        [JsonPropertyName("spread")]
        public Spread Spread { get; set; }

        [JsonPropertyName("moneyline")]
        public Moneyline Moneyline { get; set; }
    }
}
