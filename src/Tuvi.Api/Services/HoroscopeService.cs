using Microsoft.Extensions.Caching.Memory;
using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>
/// Cung cấp tử vi hằng ngày cho từng cung. Nội dung do <see cref="IReadingWriter"/> tạo,
/// còn lớp này lo phần cache theo (cung + ngày) — nên 10.000 user cùng cung/ngày chỉ tạo 1 lần.
/// Đồng thời bọc thêm lớp cá nhân hóa (mood, streak, premium, focus).
/// </summary>
public class HoroscopeService
{
    private readonly ZodiacService _zodiac;
    private readonly IReadingWriter _writer;
    private readonly IMemoryCache _cache;

    public HoroscopeService(ZodiacService zodiac, IReadingWriter writer, IMemoryCache cache)
    {
        _zodiac = zodiac;
        _writer = writer;
        _cache = cache;
    }

    public DailyHoroscope? GetDaily(string signKey, DateOnly date)
    {
        var sign = _zodiac.Get(signKey);
        if (sign is null) return null;

        string cacheKey = $"horo:{sign.Key}:{date:yyyyMMdd}";
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(26);
            return _writer.Write(sign, date);
        });
    }

    /// <summary>
    /// Bọc tử vi gốc bằng lớp cá nhân hóa: lời chào theo tâm trạng, thông điệp streak,
    /// điểm nhấn theo mối quan tâm, và "lá số chuyên sâu" nếu user là premium.
    /// </summary>
    public PersonalizedHoroscope Personalize(
        DailyHoroscope reading, string displayName, Mood? mood, int streak, bool premium, FocusArea? focus)
    {
        string d = reading.Date.ToString("yyyyMMdd");

        string greeting = mood is null
            ? "Chào bạn, cùng xem hôm nay có gì nhé!"
            : StableHash.Pick(PersonalizationContent.Greetings[mood.Value], displayName, d, "greet");

        string? deep = premium
            ? StableHash.Pick(PersonalizationContent.DeepInsights, displayName, d, reading.SignKey)
            : null;

        string? teaser = premium
            ? null
            : "🔒 Mở khóa Premium để xem 'Lá số chuyên sâu' và phân tích cặp đôi chi tiết.";

        // Làm nổi bật đúng phần user quan tâm nhất (chọn lúc onboarding).
        string? focusHighlight = focus switch
        {
            FocusArea.Love => "💗 Bạn đang quan tâm chuyện tình cảm — điểm nhấn hôm nay: " + reading.Love,
            FocusArea.Career => "📚 Bạn đang tập trung sự nghiệp/học tập — điểm nhấn hôm nay: " + reading.Career,
            FocusArea.Money => "💰 Bạn đang để ý tài chính — điểm nhấn hôm nay: " + reading.Money,
            FocusArea.Growth => "🌱 Bạn đang muốn phát triển bản thân — điểm nhấn hôm nay: " + reading.Mood,
            _ => null
        };

        return new PersonalizedHoroscope(
            DisplayName: displayName,
            Greeting: greeting,
            Mood: mood,
            Streak: streak,
            StreakMessage: PersonalizationContent.StreakMessage(streak),
            Reading: reading,
            IsPremium: premium,
            DeepInsight: deep,
            PremiumTeaser: teaser,
            FocusHighlight: focusHighlight);
    }
}
