using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL
{
    public class NFLTeam
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("abbreviation")]
        public string? Abbreviation { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("shortDisplayName")]
        public string? ShortDisplayName { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("alternateColor")]
        public string? AlternateColor { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("venue")]
        public VenueReference? Venue { get; set; }

        [JsonPropertyName("links")]
        public List<Link>? Links { get; set; }

        [JsonPropertyName("logos")]
        public List<Logo>? Logos { get; set; }

        [JsonPropertyName("record")]
        public Record? Record { get; set; }

        [JsonPropertyName("standingSummary")]
        public string? StandingSummary { get; set; }
    }
}
