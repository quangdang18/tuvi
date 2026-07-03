using Microsoft.EntityFrameworkCore;
using Tuvi.Api.Data;
using Tuvi.Api.Models;
using Tuvi.Api.Time;

namespace Tuvi.Api.Services;

/// <summary>Quản lý hồ sơ user, check-in tâm trạng, streak và tử vi cá nhân hóa.</summary>
public class UserService
{
    private readonly AppDbContext _db;
    private readonly ZodiacService _zodiac;
    private readonly NumerologyService _numerology;
    private readonly HoroscopeService _horoscope;
    private readonly IClock _clock;

    public UserService(AppDbContext db, ZodiacService zodiac, NumerologyService numerology, HoroscopeService horoscope, IClock clock)
    {
        _db = db;
        _zodiac = zodiac;
        _numerology = numerology;
        _horoscope = horoscope;
        _clock = clock;
    }

    private const int ReferralBonusDays = 3;

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
            FocusArea = req.Focus,
            ReferralCode = await GenerateReferralCodeAsync(),
            CreatedAt = _clock.Now,
        };

        // Đăng ký qua mã giới thiệu hợp lệ → thưởng premium cho cả người mời lẫn người được mời.
        User? referrer = null;
        if (!string.IsNullOrWhiteSpace(req.ReferralCode))
        {
            referrer = await _db.Users.FirstOrDefaultAsync(u => u.ReferralCode == req.ReferralCode);
            if (referrer is not null)
            {
                user.ReferredByUserId = referrer.Id;
                ExtendPremium(user, ReferralBonusDays);
            }
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (referrer is not null)
        {
            ExtendPremium(referrer, ReferralBonusDays);
            await _db.SaveChangesAsync();
        }

        return ToResult(user, zodiac);
    }

    private async Task<string> GenerateReferralCodeAsync()
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // bỏ ký tự dễ nhầm (0/O, 1/I)
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var chars = new char[6];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = alphabet[Random.Shared.Next(alphabet.Length)];
            string code = new(chars);
            if (!await _db.Users.AnyAsync(u => u.ReferralCode == code))
                return code;
        }
        return "TV" + _clock.Now.Ticks.ToString("X")[^6..];
    }

    private void ExtendPremium(User u, int days)
    {
        var from = u.PremiumUntil is { } p && p > _clock.Now ? p : _clock.Now;
        u.PremiumUntil = from.AddDays(days);
    }

    public async Task<ReferralInfo?> GetReferralAsync(int id, string baseUrl)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return null;
        int invited = await _db.Users.CountAsync(u => u.ReferredByUserId == id);
        return new ReferralInfo(user.ReferralCode, $"{baseUrl}/app.html?ref={user.ReferralCode}", invited);
    }

    public async Task<bool> SetFocusAsync(int id, FocusArea focus)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return false;
        user.FocusArea = focus;
        await _db.SaveChangesAsync();
        return true;
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
            _numerology.LifePath(u.BirthDate), u.IsPremium, u.PremiumUntil, u.ReferralCode, u.FocusArea);

    // ----- Check-in + streak + cá nhân hóa -----

    public async Task<(PersonalizedHoroscope Horoscope, StreakResult Streak)?> CheckinAsync(int id, CheckinRequest req)
    {
        var today = _clock.Today;
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

    public async Task<PersonalizedHoroscope?> GetPersonalizedAsync(int id, DateOnly? date)
    {
        var d = date ?? _clock.Today;
        var user = await _db.Users.FindAsync(id);
        if (user is null) return null;

        var checkin = await _db.Checkins.FirstOrDefaultAsync(c => c.UserId == id && c.Date == d);
        var streak = await ComputeStreakAsync(id, d);
        return BuildPersonalized(user, d, checkin?.Mood, streak.Current);
    }

    public async Task<StreakResult?> GetStreakAsync(int id)
    {
        var exists = await _db.Users.AnyAsync(u => u.Id == id);
        return exists ? await ComputeStreakAsync(id, _clock.Today) : null;
    }

    private PersonalizedHoroscope BuildPersonalized(User user, DateOnly date, Mood? mood, int streak)
    {
        var reading = _horoscope.GetDaily(user.ZodiacKey, date)!;
        return _horoscope.Personalize(reading, user.DisplayName, mood, streak, user.IsPremium, user.FocusArea);
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
