using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>Tính độ hợp giữa hai cung hoàng đạo dựa trên tương quan nguyên tố (mệnh).</summary>
public class CompatibilityService
{
    private readonly ZodiacService _zodiac;

    public CompatibilityService(ZodiacService zodiac) => _zodiac = zodiac;

    public CompatibilityResult? Evaluate(string keyA, string keyB)
    {
        var a = _zodiac.Get(keyA);
        var b = _zodiac.Get(keyB);
        if (a is null || b is null) return null;

        int basePercent = BaseHarmony(a.Element, b.Element);

        // Biến thiên nhỏ, ổn định theo cặp cụ thể để mỗi cặp có con số riêng.
        string pairKey = string.CompareOrdinal(a.Key, b.Key) <= 0 ? $"{a.Key}-{b.Key}" : $"{b.Key}-{a.Key}";
        int variance = StableHash.Compute(pairKey) % 11 - 5; // -5..+5
        int percent = Math.Clamp(basePercent + variance, 45, 99);

        var (verdict, advice) = Verdict(percent);
        return new CompatibilityResult(a.NameVi, b.NameVi, percent, verdict, ElementDetail(a, b), advice);
    }

    private static int BaseHarmony(Element x, Element y)
    {
        if (x == y) return 85;
        return Ordered(x, y) switch
        {
            (Element.Hoa, Element.Khi)  => 90, // Lửa + Gió
            (Element.Tho, Element.Thuy) => 90, // Đất + Nước
            (Element.Hoa, Element.Tho)  => 65,
            (Element.Khi, Element.Thuy) => 65,
            (Element.Hoa, Element.Thuy) => 55, // Lửa + Nước
            (Element.Tho, Element.Khi)  => 58, // Đất + Gió
            _ => 70
        };
    }

    private static (Element, Element) Ordered(Element x, Element y) =>
        (int)x <= (int)y ? (x, y) : (y, x);

    private static (string Verdict, string Advice) Verdict(int p) => p switch
    {
        >= 85 => ("Cực kỳ hợp — như định mệnh 💖", "Cứ là chính mình, hai bạn ăn ý một cách tự nhiên."),
        >= 70 => ("Rất hợp nhau 💕", "Giữ sự chân thành và dành thời gian chất lượng cho nhau."),
        >= 60 => ("Khá hợp, cần thấu hiểu 🙂", "Khác biệt là gia vị — chịu khó lắng nghe nhau nhiều hơn."),
        _     => ("Trái dấu hút nhau ⚡", "Cần kiên nhẫn và tôn trọng khác biệt, nhưng chính điều đó khiến tình cảm thú vị."),
    };

    private static string ElementDetail(ZodiacInfo a, ZodiacInfo b)
    {
        string rel = a.Element == b.Element
            ? "Cùng nguyên tố nên hai bạn dễ đồng cảm và bắt sóng nhau rất nhanh."
            : Ordered(a.Element, b.Element) switch
            {
                (Element.Hoa, Element.Khi)  => "Lửa gặp Gió — người này thổi bùng cảm hứng cho người kia, rất cuốn.",
                (Element.Tho, Element.Thuy) => "Đất gặp Nước — một người vững chãi, một người dịu dàng nuôi dưỡng, cực kỳ ăn ý.",
                (Element.Hoa, Element.Tho)  => "Lửa và Đất — cần thời gian điều chỉnh nhịp, nhưng bù trừ cho nhau tốt.",
                (Element.Khi, Element.Thuy) => "Gió và Nước — lãng mạn, bay bổng, chỉ cần thêm một chút ổn định.",
                (Element.Hoa, Element.Thuy) => "Lửa và Nước — trái ngược mà hút nhau; học cách hạ nhiệt đúng lúc là ổn.",
                (Element.Tho, Element.Khi)  => "Đất và Gió — một người thực tế, một người bay bổng; tôn trọng khác biệt sẽ hòa hợp.",
                _ => "Hai nguyên tố bổ sung cho nhau theo cách riêng."
            };
        return $"{a.NameVi} thuộc mệnh {a.ElementVi}, {b.NameVi} thuộc mệnh {b.ElementVi}. {rel}";
    }
}
