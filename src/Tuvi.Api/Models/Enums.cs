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

public enum PaymentProviderKind { MoMo, ZaloPay }

public enum PaymentStatus { Pending, Paid, Failed, Canceled }

/// <summary>Gói premium.</summary>
public enum PremiumPlan { Monthly }
