namespace Tuvi.Api.Models;

/// <summary>Nguyên tố (mệnh) của cung hoàng đạo.</summary>
public enum Element
{
    Hoa,   // Lửa
    Tho,   // Đất
    Khi,   // Khí / Gió
    Thuy   // Nước
}

/// <summary>Thông tin tĩnh của một cung hoàng đạo.</summary>
public record ZodiacInfo(
    string Key,
    string NameVi,
    string Symbol,
    Element Element,
    string DateRange,
    string RulingPlanet,
    string Keyword)
{
    /// <summary>Tên mệnh dạng tiếng Việt, dễ đọc cho người dùng.</summary>
    public string ElementVi => Element switch
    {
        Element.Hoa => "Hỏa (Lửa)",
        Element.Tho => "Thổ (Đất)",
        Element.Khi => "Khí (Gió)",
        Element.Thuy => "Thủy (Nước)",
        _ => ""
    };
}
