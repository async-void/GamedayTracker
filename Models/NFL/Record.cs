using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Record
    {
        [JsonPropertyName("items")]
        public List<RecordItem> Items { get; set; }
    }
}
