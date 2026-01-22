using System.Text.Json.Serialization;

namespace GamedayTracker.Models.News
{
    public class Image
    {
        [JsonPropertyName("dataSourceIdentifier")]
        public string DataSourceIdentifier { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        [JsonPropertyName("credit")]
        public string Credit { get; set; }

        [JsonPropertyName("alt")]
        public string Alt { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
