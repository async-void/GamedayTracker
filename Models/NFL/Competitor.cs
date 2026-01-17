using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Competitor
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("homeAway")]
        public string HomeAway { get; set; }

        [JsonPropertyName("winner")]
        public bool Winner { get; set; }

        [JsonPropertyName("team")]
        public NFLTeam Team { get; set; }

        [JsonPropertyName("score")]
        public string Score { get; set; }

        [JsonPropertyName("linescores")]
        public List<LineScore> LineScores { get; set; }

        [JsonPropertyName("statistics")]
        public List<object> Statistics { get; set; }

        [JsonPropertyName("records")]
        public List<Record> Records { get; set; }
    }
}
