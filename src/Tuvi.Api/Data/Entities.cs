using System.ComponentModel.DataAnnotations.Schema;
using Tuvi.Api.Models;

namespace Tuvi.Api.Data;

/// <summary>Hồ sơ người dùng — lưu ngày/giờ/nơi sinh để cá nhân hóa.</summary>
public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public TimeOnly? BirthTime { get; set; }
    public string? BirthPlace { get; set; }
    public string ZodiacKey { get; set; } = "";

    /// <summary>Hết hạn premium; null hoặc quá khứ = chưa/không còn premium.</summary>
    public DateTimeOffset? PremiumUntil { get; set; }

    /// <summary>Token thiết bị để gửi push (FCM/APNs). Null = chưa đăng ký nhận thông báo.</summary>
    public string? DeviceToken { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<DailyCheckin> Checkins { get; set; } = [];

    [NotMapped]
    public bool IsPremium => PremiumUntil.HasValue && PremiumUntil.Value > DateTimeOffset.UtcNow;
}

/// <summary>Check-in tâm trạng mỗi ngày — nền tảng cho streak và cá nhân hóa.</summary>
public class DailyCheckin
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateOnly Date { get; set; }
    public Mood Mood { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Đơn thanh toán để mở khóa premium.</summary>
public class PaymentOrder
{
    public int Id { get; set; }
    public string OrderId { get; set; } = "";
    public int UserId { get; set; }
    public PaymentProviderKind Provider { get; set; }
    public long Amount { get; set; }
    public PremiumPlan Plan { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ProviderTransId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
}
