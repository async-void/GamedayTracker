using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.API
{
    public class Datum
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("visitor_team")]
        public VisitorTeam visitor_team { get; set; }

        [JsonPropertyName("home_team")]
        public HomeTeam home_team { get; set; }

        [JsonPropertyName("summary")]
        public string summary { get; set; }

        [JsonPropertyName("venue")]
        public string venue { get; set; }

        [JsonPropertyName("week")]
        public int week { get; set; }

        [JsonPropertyName("date")]
        public DateTime date { get; set; }

        [JsonPropertyName("season")]
        public int season { get; set; }

        [JsonPropertyName("postseason")]
        public bool postseason { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("home_team_score")]
        public int home_team_score { get; set; }

        [JsonPropertyName("home_team_q1")]
        public int home_team_q1 { get; set; }

        [JsonPropertyName("home_team_q2")]
        public int home_team_q2 { get; set; }

        [JsonPropertyName("home_team_q3")]
        public int home_team_q3 { get; set; }

        [JsonPropertyName("home_team_q4")]
        public int home_team_q4 { get; set; }

        [JsonPropertyName("home_team_ot")]
        public object home_team_ot { get; set; }

        [JsonPropertyName("visitor_team_score")]
        public int visitor_team_score { get; set; }

        [JsonPropertyName("visitor_team_q1")]
        public int visitor_team_q1 { get; set; }

        [JsonPropertyName("visitor_team_q2")]
        public int visitor_team_q2 { get; set; }

        [JsonPropertyName("visitor_team_q3")]
        public object visitor_team_q3 { get; set; }

        [JsonPropertyName("visitor_team_q4")]
        public int visitor_team_q4 { get; set; }

        [JsonPropertyName("visitor_team_ot")]
        public object visitor_team_ot { get; set; }
    }
}
