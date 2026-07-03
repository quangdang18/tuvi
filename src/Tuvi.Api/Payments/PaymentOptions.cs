namespace Tuvi.Api.Payments;

/// <summary>
/// Cấu hình thanh toán (đọc từ appsettings "Payment").
/// Mode = "Mock" (mặc định) để chạy local không cần merchant key; "Live" để gọi cổng thật.
/// </summary>
public class PaymentOptions
{
    public string Mode { get; set; } = "Mock";
    public long MonthlyPriceVnd { get; set; } = 20000;

    public MoMoOptions MoMo { get; set; } = new();
    public ZaloPayOptions ZaloPay { get; set; } = new();

    public bool IsLive => string.Equals(Mode, "Live", StringComparison.OrdinalIgnoreCase);
}

public class MoMoOptions
{
    public string PartnerCode { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
}

public class ZaloPayOptions
{
    public string AppId { get; set; } = "";
    public string Key1 { get; set; } = "";
    public string Key2 { get; set; } = "";
    public string Endpoint { get; set; } = "https://sb-openapi.zalopay.vn/v2/create";
}
