using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Venue
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("address")]
        public Address Address { get; set; }

        [JsonPropertyName("indoor")]
        public bool Indoor { get; set; }
    }
}
