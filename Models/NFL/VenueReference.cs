using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class VenueReference
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
