using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class ConnectionStrings
    {
        [JsonPropertyName("default")]
        public string? Default { get; set; }
        [JsonPropertyName("cloud")]
        public string? Cloud { get; set; }
        [JsonPropertyName("gameday")] 
        public string? Gameday { get; set; }
    }
}
