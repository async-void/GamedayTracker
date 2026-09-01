namespace GamedayTracker.Models.DailyNumbers
{
    public sealed record DailyNumberPick(ulong GuildId,ulong UserId,DateOnly Date,IReadOnlyList<int> Numbers,string PlayType,DateTimeOffset Timestamp);

}
