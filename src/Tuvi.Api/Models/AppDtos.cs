namespace Tuvi.Api.Models;

// ----- User / hồ sơ -----

public record RegisterUserRequest(
    string DisplayName,
    DateOnly BirthDate,
    TimeOnly? BirthTime,
    string? BirthPlace);

public record UserResult(
    int Id,
    string DisplayName,
    DateOnly BirthDate,
    TimeOnly? BirthTime,
    string? BirthPlace,
    ZodiacInfo Zodiac,
    NumerologyResult Numerology,
    bool IsPremium,
    DateTimeOffset? PremiumUntil);

public record DeviceTokenRequest(string Token);

// ----- Check-in mood + streak -----

public record CheckinRequest(Mood Mood, string? Note);

public record StreakResult(
    int Current,
    int Longest,
    DateOnly? LastCheckinDate,
    bool CheckedInToday);

public record CheckinResponse(PersonalizedHoroscope Horoscope, StreakResult Streak);

// ----- Tử vi cá nhân hóa -----

public record PersonalizedHoroscope(
    string DisplayName,
    string Greeting,
    Mood? Mood,
    int Streak,
    string StreakMessage,
    DailyHoroscope Reading,
    bool IsPremium,
    string? DeepInsight,
    string? PremiumTeaser);

// ----- Thanh toán -----

public record PaymentCreateRequest(int UserId, PaymentProviderKind Provider, PremiumPlan Plan);

public record PaymentCreateResult(
    string OrderId,
    PaymentProviderKind Provider,
    long Amount,
    string PayUrl,
    PaymentStatus Status,
    string Mode);

public record SimulateRequest(string OrderId);

public record PaymentStatusResult(
    string OrderId,
    PaymentStatus Status,
    long Amount,
    PaymentProviderKind Provider,
    DateTimeOffset? PremiumUntil);
