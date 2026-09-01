using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class NflStandings
    {
        [JsonPropertyName("season")]
        public Season? Season { get; set; }
        [JsonPropertyName("children")]
        public List<Conference>? Children { get; set; }
    }
}
