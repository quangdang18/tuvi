using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>
/// Nguồn tạo nội dung tử vi thô cho (cung, ngày).
/// Tách riêng để có thể đổi từ template (chi phí ~0) sang AI về sau
/// mà không đụng tới lớp cache trong <see cref="HoroscopeService"/>.
/// </summary>
public interface IReadingWriter
{
    DailyHoroscope Write(ZodiacInfo sign, DateOnly date);
}

/// <summary>
/// Bản mặc định: ghép câu từ <see cref="HoroscopeContent"/> theo băm ổn định (cung + ngày).
/// Deterministic → nội dung cố định trong ngày, cache được, không tốn tiền AI.
/// </summary>
public class TemplateReadingWriter : IReadingWriter
{
    public DailyHoroscope Write(ZodiacInfo sign, DateOnly date)
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
