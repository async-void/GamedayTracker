using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL
{
    public sealed class TeamRoot
    {
        [JsonPropertyName("JSON")]
        public TeamWrapper? Json { get; set; }
    }
}
