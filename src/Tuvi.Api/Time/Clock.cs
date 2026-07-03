namespace Tuvi.Api.Time;

/// <summary>Nguồn thời gian chuẩn (giờ Việt Nam) — tách ra để test được và tránh lẫn UTC/local.</summary>
public interface IClock
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}

/// <summary>Đồng hồ hệ thống quy về múi giờ Việt Nam (Asia/Ho_Chi_Minh, UTC+7).</summary>
public class SystemClock : IClock
{
    private static readonly TimeZoneInfo Vn = Resolve();

    public DateTimeOffset Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Vn);
    public DateOnly Today => DateOnly.FromDateTime(Now.DateTime);

    private static TimeZoneInfo Resolve()
    {
        // IANA trên Linux/macOS, Windows-id trên Windows — thử cả hai.
        foreach (var id in new[] { "Asia/Ho_Chi_Minh", "SE Asia Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.CreateCustomTimeZone("VN", TimeSpan.FromHours(7), "Vietnam", "Vietnam");
    }
}

/// <summary>Đồng hồ cố định cho unit test.</summary>
public class FixedClock : IClock
{
    public FixedClock(DateOnly today) => Today = today;
    public DateOnly Today { get; }
    public DateTimeOffset Now => new(Today.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
}
