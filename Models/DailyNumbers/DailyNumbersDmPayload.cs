namespace GamedayTracker.Models.DailyNumbers
{
    public sealed record DailyNumbersDmPayload(
        ulong UserId,
        int[] Pick,
        int[] WinningNumber,
        decimal Payout
    );

}
