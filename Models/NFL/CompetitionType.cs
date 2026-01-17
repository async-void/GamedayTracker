using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class CompetitionType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }
    }
}
