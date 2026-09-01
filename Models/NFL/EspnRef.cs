using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public sealed class EspnRef
    {
        [JsonPropertyName("$ref")]
        public string Ref { get; set; } = string.Empty;
    }

}
