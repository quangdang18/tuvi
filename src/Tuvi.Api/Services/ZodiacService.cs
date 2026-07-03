using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>Tra cứu 12 cung hoàng đạo và suy cung từ ngày sinh.</summary>
public class ZodiacService
{
    private static readonly ZodiacInfo[] All =
    [
        new("aries",       "Bạch Dương", "♈", Element.Hoa,  "21/3 – 19/4",   "Sao Hỏa",         "nhiệt huyết, tiên phong"),
        new("taurus",      "Kim Ngưu",   "♉", Element.Tho,  "20/4 – 20/5",   "Sao Kim",         "kiên định, thực tế"),
        new("gemini",      "Song Tử",    "♊", Element.Khi,  "21/5 – 20/6",   "Sao Thủy",        "linh hoạt, tò mò"),
        new("cancer",      "Cự Giải",    "♋", Element.Thuy, "21/6 – 22/7",   "Mặt Trăng",       "tình cảm, chở che"),
        new("leo",         "Sư Tử",      "♌", Element.Hoa,  "23/7 – 22/8",   "Mặt Trời",        "tự tin, hào phóng"),
        new("virgo",       "Xử Nữ",      "♍", Element.Tho,  "23/8 – 22/9",   "Sao Thủy",        "tỉ mỉ, cầu toàn"),
        new("libra",       "Thiên Bình", "♎", Element.Khi,  "23/9 – 22/10",  "Sao Kim",         "hài hòa, công bằng"),
        new("scorpio",     "Bọ Cạp",     "♏", Element.Thuy, "23/10 – 21/11", "Sao Diêm Vương",  "sâu sắc, mãnh liệt"),
        new("sagittarius", "Nhân Mã",    "♐", Element.Hoa,  "22/11 – 21/12", "Sao Mộc",         "tự do, phiêu lưu"),
        new("capricorn",   "Ma Kết",     "♑", Element.Tho,  "22/12 – 19/1",  "Sao Thổ",         "tham vọng, kỷ luật"),
        new("aquarius",    "Bảo Bình",   "♒", Element.Khi,  "20/1 – 18/2",   "Sao Thiên Vương", "sáng tạo, khác biệt"),
        new("pisces",      "Song Ngư",   "♓", Element.Thuy, "19/2 – 20/3",   "Sao Hải Vương",   "mộng mơ, nhạy cảm"),
    ];

    private static readonly Dictionary<string, ZodiacInfo> ByKey =
        All.ToDictionary(z => z.Key, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<ZodiacInfo> GetAll() => All;

    public ZodiacInfo? Get(string key) =>
        key is not null && ByKey.TryGetValue(key, out var z) ? z : null;

    /// <summary>Suy ra cung hoàng đạo từ ngày sinh theo ranh giới ngày chuẩn.</summary>
    public ZodiacInfo GetByDate(DateOnly date)
    {
        string key = (date.Month, date.Day) switch
        {
            (1, <= 19) => "capricorn",
            (1, _)     => "aquarius",
            (2, <= 18) => "aquarius",
            (2, _)     => "pisces",
            (3, <= 20) => "pisces",
            (3, _)     => "aries",
            (4, <= 19) => "aries",
            (4, _)     => "taurus",
            (5, <= 20) => "taurus",
            (5, _)     => "gemini",
            (6, <= 20) => "gemini",
            (6, _)     => "cancer",
            (7, <= 22) => "cancer",
            (7, _)     => "leo",
            (8, <= 22) => "leo",
            (8, _)     => "virgo",
            (9, <= 22) => "virgo",
            (9, _)     => "libra",
            (10, <= 22) => "libra",
            (10, _)     => "scorpio",
            (11, <= 21) => "scorpio",
            (11, _)     => "sagittarius",
            (12, <= 21) => "sagittarius",
            (12, _)     => "capricorn",
            _ => "capricorn"
        };
        return ByKey[key];
    }
}
