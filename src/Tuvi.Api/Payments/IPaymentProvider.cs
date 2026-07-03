using System.Security.Cryptography;
using System.Text;
using Tuvi.Api.Data;
using Tuvi.Api.Models;

namespace Tuvi.Api.Payments;

/// <summary>Trừu tượng cho một cổng thanh toán (MoMo, ZaloPay...).</summary>
public interface IPaymentProvider
{
    PaymentProviderKind Kind { get; }

    /// <summary>Sinh mã đơn hợp lệ theo định dạng của cổng.</summary>
    string NewOrderId();

    /// <summary>Tạo link thanh toán. Mock: trả trang mô phỏng local; Live: gọi API cổng thật.</summary>
    Task<string> CreatePayUrlAsync(PaymentOrder order, string returnBaseUrl);

    /// <summary>Xác minh callback/IPN từ cổng. Trả về true nếu chữ ký hợp lệ.</summary>
    bool VerifyCallback(IReadOnlyDictionary<string, string> fields, out bool success, out string? transId);
}

/// <summary>Tiện ích chữ ký HMAC-SHA256 (hex thường) dùng chung cho các cổng.</summary>
public static class Crypto
{
    public static string HmacSha256(string data, string key)
    {
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(h.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLowerInvariant();
    }
}

internal static class MockPayUrl
{
    public static string Build(string baseUrl, PaymentOrder order) =>
        $"{baseUrl}/pay-mock.html?orderId={Uri.EscapeDataString(order.OrderId)}&amount={order.Amount}&provider={order.Provider}";
}
