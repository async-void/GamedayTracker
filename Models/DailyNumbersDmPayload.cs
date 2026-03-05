namespace GamedayTracker.Models
{
    public sealed record DailyNumbersDmPayload(
        ulong UserId,
        int[] Pick,
        int[] WinningNumber,
        decimal Payout
    );

}
