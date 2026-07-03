using Microsoft.Extensions.Caching.Memory;
using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>
/// Sinh tử vi hằng ngày cho từng cung. Nội dung được chọn deterministic theo (cung + ngày)
/// và cache lại — nên 10.000 user cùng cung/ngày chỉ tạo 1 lần, chi phí gần như bằng 0.
/// </summary>
public class HoroscopeService
{
    private readonly ZodiacService _zodiac;
    private readonly IMemoryCache _cache;

    public HoroscopeService(ZodiacService zodiac, IMemoryCache cache)
    {
        _zodiac = zodiac;
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
            return Build(sign, date);
        });
    }

    /// <summary>
    /// Bọc tử vi gốc bằng lớp cá nhân hóa: lời chào theo tâm trạng, thông điệp streak,
    /// và "lá số chuyên sâu" nếu user là premium (nếu không thì trả teaser để mời nâng cấp).
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

    private static DailyHoroscope Build(ZodiacInfo sign, DateOnly date)
    {
        string d = date.ToString("yyyyMMdd");
        string k = sign.Key;

        int score = 3 + StableHash.Pick(3, k, d, "score");        // 3..5 sao (thiên tích cực)
        int lucky = StableHash.Compute($"{k}|{d}|lucky") % 100;    // 0..99

        return new DailyHoroscope(
            SignKey: sign.Key,
            SignNameVi: sign.NameVi,
            Symbol: sign.Symbol,
            Date: date,
            Score: score,
            Headline: StableHash.Pick(HoroscopeContent.Headlines, k, d, "head"),
            Overall: StableHash.Pick(HoroscopeContent.Overall, k, d, "overall"),
            Love: StableHash.Pick(HoroscopeContent.Love, k, d, "love"),
            Career: StableHash.Pick(HoroscopeContent.Career, k, d, "career"),
            Money: StableHash.Pick(HoroscopeContent.Money, k, d, "money"),
            Mood: StableHash.Pick(HoroscopeContent.Mood, k, d, "mood"),
            Advice: StableHash.Pick(HoroscopeContent.Advice, k, d, "advice"),
            LuckyNumber: lucky,
            LuckyColor: StableHash.Pick(HoroscopeContent.LuckyColors, k, d, "color"));
    }
}
