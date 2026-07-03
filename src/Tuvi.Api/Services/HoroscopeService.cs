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
