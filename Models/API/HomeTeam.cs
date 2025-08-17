using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.API
{
    public class HomeTeam
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("conference")]
        public string conference { get; set; }

        [JsonPropertyName("division")]
        public string division { get; set; }

        [JsonPropertyName("location")]
        public string location { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("full_name")]
        public string full_name { get; set; }

        [JsonPropertyName("abbreviation")]
        public string abbreviation { get; set; }
    }
}
