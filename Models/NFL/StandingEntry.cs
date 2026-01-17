namespace GamedayTracker.Models.NFL
{
    public class StandingEntry
    {
        public NFLTeam Team { get; set; }
        public List<Stat> Stats { get; set; }
    }
}
