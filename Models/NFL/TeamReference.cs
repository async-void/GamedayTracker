using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class TeamReference
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
