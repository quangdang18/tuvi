namespace Tuvi.Api.Models;

/// <summary>Tâm trạng người dùng tự khai mỗi ngày (dùng để cá nhân hóa nội dung).</summary>
public enum Mood
{
    Happy,     // Vui
    Normal,    // Bình thường
    Sad,       // Buồn
    Stressed,  // Căng thẳng
    Excited,   // Hào hứng
    Tired      // Mệt mỏi
}

/// <summary>Mối quan tâm chính người dùng chọn khi onboarding — dùng để làm nổi bật đúng phần họ quan tâm.</summary>
public enum FocusArea
{
    Love,    // Tình cảm
    Career,  // Sự nghiệp / học tập
    Money,   // Tài chính
    Growth   // Phát triển bản thân
}

public enum PaymentProviderKind { MoMo, ZaloPay }

public enum PaymentStatus { Pending, Paid, Failed, Canceled }

/// <summary>Gói premium.</summary>
public enum PremiumPlan { Monthly }
