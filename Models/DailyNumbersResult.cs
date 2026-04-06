namespace GamedayTracker.Models
{
    public sealed record DailyNumbersResult(
        ulong UserId,
        ulong GuildId,
        IReadOnlyList<int> Pick,
        int[] WinningNumber,
        int MatchCount,
        decimal Payout,
        string PlayType
    );

}
