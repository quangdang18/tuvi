using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tuvi.Api.Data;
using Tuvi.Api.Services;

namespace Tuvi.Api.Push;

/// <summary>
/// Nền: mỗi sáng vào giờ cấu hình, gửi tử vi hôm nay cho mọi user đã đăng ký device token.
/// Đây là "cú hích" kéo user quay lại mỗi ngày (giữ streak, chống churn).
/// </summary>
public class DailyHoroscopePushJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PushOptions _opt;
    private readonly ILogger<DailyHoroscopePushJob> _log;

    public DailyHoroscopePushJob(IServiceScopeFactory scopeFactory, IOptions<PushOptions> opt, ILogger<DailyHoroscopePushJob> log)
    {
        _scopeFactory = scopeFactory;
        _opt = opt.Value;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeUntilNextRun();
            _log.LogInformation("Push buổi sáng kế tiếp sau {Delay}", delay);
            try { await Task.Delay(delay, stoppingToken); }
            catch (TaskCanceledException) { break; }

            try { await SendDailyAsync(stoppingToken); }
            catch (Exception ex) { _log.LogError(ex, "Lỗi khi gửi push buổi sáng"); }
        }
    }

    private TimeSpan TimeUntilNextRun()
    {
        var now = DateTime.Now;
        var next = now.Date.AddHours(_opt.SendAtHour);
        if (next <= now) next = next.AddDays(1);
        return next - now;
    }

    /// <summary>Gửi push cho tất cả user có device token. Public để endpoint dev gọi thử ngay.</summary>
    public async Task<int> SendDailyAsync(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var horoscope = scope.ServiceProvider.GetRequiredService<HoroscopeService>();
        var sender = scope.ServiceProvider.GetRequiredService<IPushSender>();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var users = await db.Users.Where(u => u.DeviceToken != null).ToListAsync(ct);

        int sent = 0;
        foreach (var u in users)
        {
            var reading = horoscope.GetDaily(u.ZodiacKey, today);
            if (reading is null) continue;
            string title = $"{reading.Symbol} {reading.SignNameVi} hôm nay";
            await sender.SendAsync(u.DeviceToken!, title, reading.Headline, ct);
            sent++;
        }

        _log.LogInformation("Đã gửi push buổi sáng cho {Count} user", sent);
        return sent;
    }
}
