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
    WeeklyHoroscope WriteWeekly(ZodiacInfo sign, DateOnly weekStart, DateOnly weekEnd);
    MonthlyHoroscope WriteMonthly(ZodiacInfo sign, int year, int month);
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

    public WeeklyHoroscope WriteWeekly(ZodiacInfo sign, DateOnly weekStart, DateOnly weekEnd)
    {
        string w = weekStart.ToString("yyyyMMdd");
        string k = sign.Key;
        int score = 3 + StableHash.Pick(3, k, w, "wscore");

        return new WeeklyHoroscope(
            SignKey: sign.Key,
            SignNameVi: sign.NameVi,
            Symbol: sign.Symbol,
            WeekStart: weekStart,
            WeekEnd: weekEnd,
            Score: score,
            Headline: StableHash.Pick(WeeklyMonthlyContent.WeekHeadlines, k, w, "whead"),
            Overall: StableHash.Pick(WeeklyMonthlyContent.WeekOverall, k, w, "wover"),
            Love: StableHash.Pick(WeeklyMonthlyContent.WeekLove, k, w, "wlove"),
            Career: StableHash.Pick(WeeklyMonthlyContent.WeekCareer, k, w, "wcareer"),
            Money: StableHash.Pick(WeeklyMonthlyContent.WeekMoney, k, w, "wmoney"),
            LuckyDay: StableHash.Pick(WeeklyMonthlyContent.Weekdays, k, w, "wday"));
    }

    public MonthlyHoroscope WriteMonthly(ZodiacInfo sign, int year, int month)
    {
        string ym = $"{year}{month:D2}";
        string k = sign.Key;
        int score = 3 + StableHash.Pick(3, k, ym, "mscore");

        // Vài ngày đẹp trong tháng (deterministic, không trùng).
        int days = DateTime.DaysInMonth(year, month);
        var luckyDays = new SortedSet<int>();
        for (int salt = 0; luckyDays.Count < 3; salt++)
            luckyDays.Add(1 + StableHash.Compute($"{k}|{ym}|md{salt}") % days);

        return new MonthlyHoroscope(
            SignKey: sign.Key,
            SignNameVi: sign.NameVi,
            Symbol: sign.Symbol,
            Year: year,
            Month: month,
            Score: score,
            Theme: StableHash.Pick(WeeklyMonthlyContent.MonthThemes, k, ym, "theme"),
            Overall: StableHash.Pick(WeeklyMonthlyContent.MonthOverall, k, ym, "mover"),
            Love: StableHash.Pick(WeeklyMonthlyContent.MonthLove, k, ym, "mlove"),
            Career: StableHash.Pick(WeeklyMonthlyContent.MonthCareer, k, ym, "mcareer"),
            Money: StableHash.Pick(WeeklyMonthlyContent.MonthMoney, k, ym, "mmoney"),
            LuckyDates: string.Join(", ", luckyDays));
    }
}
