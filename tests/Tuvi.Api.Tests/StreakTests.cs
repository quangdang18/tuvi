using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tuvi.Api.Data;
using Tuvi.Api.Models;
using Tuvi.Api.Services;
using Tuvi.Api.Time;
using Xunit;

namespace Tuvi.Api.Tests;

public class StreakTests
{
    private static (UserService Svc, AppDbContext Db, int UserId) Build(DateOnly today, params DateOnly[] checkinDates)
    {
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var user = new User
        {
            DisplayName = "Test",
            BirthDate = new DateOnly(2001, 8, 1),
            ZodiacKey = "leo",
            CreatedAt = DateTimeOffset.UtcNow,
        };
        db.Users.Add(user);
        db.SaveChanges();

        foreach (var d in checkinDates)
            db.Checkins.Add(new DailyCheckin { UserId = user.Id, Date = d, Mood = Mood.Normal, CreatedAt = DateTimeOffset.UtcNow });
        db.SaveChanges();

        var horo = new HoroscopeService(new ZodiacService(), new MemoryCache(new MemoryCacheOptions()));
        var svc = new UserService(db, new ZodiacService(), new NumerologyService(), horo, new FixedClock(today));
        return (svc, db, user.Id);
    }

    [Fact]
    public async Task Streak_counts_consecutive_days_including_today()
    {
        var today = new DateOnly(2026, 7, 3);
        var (svc, _, id) = Build(today, today, today.AddDays(-1), today.AddDays(-2));
        var s = await svc.GetStreakAsync(id);
        Assert.Equal(3, s!.Current);
        Assert.Equal(3, s.Longest);
        Assert.True(s.CheckedInToday);
    }

    [Fact]
    public async Task Streak_counts_from_yesterday_when_not_checked_in_today()
    {
        var today = new DateOnly(2026, 7, 3);
        var (svc, _, id) = Build(today, today.AddDays(-1), today.AddDays(-2));
        var s = await svc.GetStreakAsync(id);
        Assert.Equal(2, s!.Current);
        Assert.False(s.CheckedInToday);
    }

    [Fact]
    public async Task Streak_is_zero_without_checkins()
    {
        var (svc, _, id) = Build(new DateOnly(2026, 7, 3));
        var s = await svc.GetStreakAsync(id);
        Assert.Equal(0, s!.Current);
        Assert.Equal(0, s.Longest);
    }

    [Fact]
    public async Task Longest_streak_tracks_best_run_even_after_a_gap()
    {
        var today = new DateOnly(2026, 7, 10);
        var (svc, _, id) = Build(today,
            today, today.AddDays(-1),                                 // run hiện tại = 2
            today.AddDays(-5), today.AddDays(-6), today.AddDays(-7)); // run tốt nhất = 3
        var s = await svc.GetStreakAsync(id);
        Assert.Equal(2, s!.Current);
        Assert.Equal(3, s.Longest);
    }

    [Fact]
    public async Task Checkin_twice_same_day_is_idempotent()
    {
        var today = new DateOnly(2026, 7, 3);
        var (svc, db, id) = Build(today);
        await svc.CheckinAsync(id, new CheckinRequest(Mood.Happy, null));
        var second = await svc.CheckinAsync(id, new CheckinRequest(Mood.Sad, null));
        Assert.Equal(1, second!.Value.Streak.Current);
        Assert.Equal(1, db.Checkins.Count(c => c.UserId == id));
    }
}
