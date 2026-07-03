namespace Tuvi.Api.Push;

public interface IPushSender
{
    Task SendAsync(string deviceToken, string title, string body, CancellationToken ct = default);
}

/// <summary>
/// Bản gửi push cho local: ghi ra log + lưu vào <see cref="PushLog"/> để xem lại.
/// Khi triển khai thật, thay bằng bản gọi FCM/APNs (giữ nguyên interface).
/// </summary>
public class LogPushSender : IPushSender
{
    private readonly ILogger<LogPushSender> _log;
    private readonly PushLog _pushLog;

    public LogPushSender(ILogger<LogPushSender> log, PushLog pushLog)
    {
        _log = log;
        _pushLog = pushLog;
    }

    public Task SendAsync(string deviceToken, string title, string body, CancellationToken ct = default)
    {
        _log.LogInformation("PUSH → {Token}: {Title} — {Body}", Trunc(deviceToken), title, body);
        _pushLog.Add(new PushRecord(deviceToken, title, body, DateTimeOffset.UtcNow));
        return Task.CompletedTask;
    }

    private static string Trunc(string s) => s.Length <= 12 ? s : s[..12] + "…";
}
