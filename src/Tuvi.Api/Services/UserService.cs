using Microsoft.EntityFrameworkCore;
using Tuvi.Api.Data;
using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>Quản lý hồ sơ user, check-in tâm trạng, streak và tử vi cá nhân hóa.</summary>
public class UserService
{
    private readonly AppDbContext _db;
    private readonly ZodiacService _zodiac;
    private readonly NumerologyService _numerology;
    private readonly HoroscopeService _horoscope;

    public UserService(AppDbContext db, ZodiacService zodiac, NumerologyService numerology, HoroscopeService horoscope)
    {
        _db = db;
        _zodiac = zodiac;
        _numerology = numerology;
        _horoscope = horoscope;
    }

    public async Task<UserResult> RegisterAsync(RegisterUserRequest req)
    {
        var zodiac = _zodiac.GetByDate(req.BirthDate);
        var user = new User
        {
            DisplayName = string.IsNullOrWhiteSpace(req.DisplayName) ? "Bạn" : req.DisplayName.Trim(),
            BirthDate = req.BirthDate,
            BirthTime = req.BirthTime,
            BirthPlace = req.BirthPlace,
            ZodiacKey = zodiac.Key,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return ToResult(user, zodiac);
    }

    public async Task<UserResult?> GetAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        return user is null ? null : ToResult(user, _zodiac.Get(user.ZodiacKey)!);
    }

    public async Task<bool> SetDeviceTokenAsync(int id, string token)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return false;
        user.DeviceToken = token;
        await _db.SaveChangesAsync();
        return true;
    }

    private UserResult ToResult(User u, ZodiacInfo z) =>
        new(u.Id, u.DisplayName, u.BirthDate, u.BirthTime, u.BirthPlace, z,
            _numerology.LifePath(u.BirthDate), u.IsPremium, u.PremiumUntil);

    // ----- Check-in + streak + cá nhân hóa -----

    public async Task<(PersonalizedHoroscope Horoscope, StreakResult Streak)?> CheckinAsync(int id, CheckinRequest req, DateOnly today)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return null;

        var existing = await _db.Checkins.FirstOrDefaultAsync(c => c.UserId == id && c.Date == today);
        if (existing is null)
            _db.Checkins.Add(new DailyCheckin { UserId = id, Date = today, Mood = req.Mood, Note = req.Note, CreatedAt = DateTimeOffset.UtcNow });
        else
            (existing.Mood, existing.Note) = (req.Mood, req.Note);

        await _db.SaveChangesAsync();

        var streak = await ComputeStreakAsync(id, today);
        var horo = BuildPersonalized(user, today, req.Mood, streak.Current);
        return (horo, streak);
    }

    public async Task<PersonalizedHoroscope?> GetPersonalizedAsync(int id, DateOnly date)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return null;

        var checkin = await _db.Checkins.FirstOrDefaultAsync(c => c.UserId == id && c.Date == date);
        var streak = await ComputeStreakAsync(id, date);
        return BuildPersonalized(user, date, checkin?.Mood, streak.Current);
    }

    public async Task<StreakResult?> GetStreakAsync(int id, DateOnly today)
    {
        var exists = await _db.Users.AnyAsync(u => u.Id == id);
        return exists ? await ComputeStreakAsync(id, today) : null;
    }

    private PersonalizedHoroscope BuildPersonalized(User user, DateOnly date, Mood? mood, int streak)
    {
        var reading = _horoscope.GetDaily(user.ZodiacKey, date)!;
        return _horoscope.Personalize(reading, user.DisplayName, mood, streak, user.IsPremium);
    }

    private async Task<StreakResult> ComputeStreakAsync(int userId, DateOnly today)
    {
        var dates = await _db.Checkins
            .Where(c => c.UserId == userId)
            .Select(c => c.Date)
            .ToListAsync();

        if (dates.Count == 0)
            return new StreakResult(0, 0, null, false);

        var set = dates.ToHashSet();
        bool checkedInToday = set.Contains(today);

        // Chuỗi hiện tại: đếm lùi liên tiếp từ hôm nay (hoặc hôm qua nếu chưa check-in hôm nay).
        int current = 0;
        var cursor = checkedInToday ? today : today.AddDays(-1);
        while (set.Contains(cursor)) { current++; cursor = cursor.AddDays(-1); }

        // Chuỗi dài nhất trong lịch sử.
        int longest = 0, run = 0;
        DateOnly? prev = null;
        foreach (var d in dates.OrderBy(x => x))
        {
            if (prev is null || d == prev.Value.AddDays(1)) run++;
            else if (d != prev.Value) run = 1;
            longest = Math.Max(longest, run);
            prev = d;
        }

        return new StreakResult(current, longest, set.Max(), checkedInToday);
    }
}
