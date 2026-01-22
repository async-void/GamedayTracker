using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Competition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("attendance")]
        public int Attendance { get; set; }

        [JsonPropertyName("type")]
        public CompetitionType Type { get; set; }

        [JsonPropertyName("timeValid")]
        public bool TimeValid { get; set; }

        [JsonPropertyName("neutralSite")]
        public bool NeutralSite { get; set; }

        [JsonPropertyName("conferenceCompetition")]
        public bool ConferenceCompetition { get; set; }

        [JsonPropertyName("playByPlayAvailable")]
        public bool PlayByPlayAvailable { get; set; }

        [JsonPropertyName("recent")]
        public bool Recent { get; set; }

        [JsonPropertyName("venue")]
        public Venue Venue { get; set; }

        [JsonPropertyName("competitors")]
        public List<Competitor> Competitors { get; set; }

        [JsonPropertyName("notes")]
        public List<Note> Notes { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }

        //[JsonPropertyName("broadcasts")]
        //public List<Broadcast> Broadcasts { get; set; }

        [JsonPropertyName("leaders")]
        public List<Leader> Leaders { get; set; }

        [JsonPropertyName("format")]
        public Format Format { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("broadcast")]
        public string Broadcast { get; set; }

        [JsonPropertyName("geoBroadcasts")]
        public List<GeoBroadcast> GeoBroadcasts { get; set; }

        [JsonPropertyName("headlines")]
        public List<Headline> Headlines { get; set; }

        [JsonPropertyName("highlights")]
        public List<object> Highlights { get; set; }
    }
}
