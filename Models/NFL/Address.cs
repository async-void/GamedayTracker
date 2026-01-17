using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Address
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
