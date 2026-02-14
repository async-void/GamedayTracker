namespace GamedayTracker.Models.Betting
{
    public class BetPayoutResult
    {
        public decimal Multiplier { get; set; }
        public decimal Winnings { get; set; }
        public decimal TotalPayout { get; set; } // winnings + original stake
    }
}
