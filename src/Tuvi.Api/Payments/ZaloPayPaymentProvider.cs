using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tuvi.Api.Data;
using Tuvi.Api.Models;

namespace Tuvi.Api.Payments;

/// <summary>Cổng ZaloPay (API v2 create + callback). Ở Mock mode trả về trang thanh toán mô phỏng.</summary>
public class ZaloPayPaymentProvider : IPaymentProvider
{
    private readonly PaymentOptions _opt;
    private readonly IHttpClientFactory _http;

    public ZaloPayPaymentProvider(IOptions<PaymentOptions> opt, IHttpClientFactory http)
    {
        _opt = opt.Value;
        _http = http;
    }

    public PaymentProviderKind Kind => PaymentProviderKind.ZaloPay;

    // ZaloPay yêu cầu app_trans_id dạng yyMMdd_xxxxx.
    public string NewOrderId()
    {
        var now = DateTime.Now;
        return $"{now:yyMMdd}_{Guid.NewGuid().ToString("N")[..10]}";
    }

    public async Task<string> CreatePayUrlAsync(PaymentOrder order, string returnBaseUrl)
    {
        if (!_opt.IsLive)
            return MockPayUrl.Build(returnBaseUrl, order);

        var z = _opt.ZaloPay;
        long appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        string appUser = $"user{order.UserId}";
        string embedData = $"{{\"redirecturl\":\"{returnBaseUrl}/payment-return.html\"}}";
        const string item = "[]";

        // data ký: app_id|app_trans_id|app_user|amount|app_time|embed_data|item  (HMAC key1)
        string macData = $"{z.AppId}|{order.OrderId}|{appUser}|{order.Amount}|{appTime}|{embedData}|{item}";
        string mac = Crypto.HmacSha256(macData, z.Key1);

        var form = new Dictionary<string, string>
        {
            ["app_id"] = z.AppId,
            ["app_trans_id"] = order.OrderId,
            ["app_user"] = appUser,
            ["app_time"] = appTime.ToString(),
            ["amount"] = order.Amount.ToString(),
            ["item"] = item,
            ["embed_data"] = embedData,
            ["description"] = "Nang cap Premium Tu Vi",
            ["bank_code"] = "",
            ["callback_url"] = $"{returnBaseUrl}/api/payment/zalopay/callback",
            ["mac"] = mac
        };

        var resp = await _http.CreateClient().PostAsync(z.Endpoint, new FormUrlEncodedContent(form));
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("order_url", out var url) ? url.GetString() ?? "" : "";
    }

    public bool VerifyCallback(IReadOnlyDictionary<string, string> f, out bool success, out string? transId)
    {
        success = false;
        transId = null;
        var z = _opt.ZaloPay;

        // ZaloPay gửi { data, mac }; mac = HMAC(key2, data).
        string data = f.TryGetValue("data", out var d) ? d : "";
        string mac = f.TryGetValue("mac", out var mm) ? mm : "";
        string expected = Crypto.HmacSha256(data, z.Key2);

        if (!string.Equals(expected, mac, StringComparison.OrdinalIgnoreCase))
            return false;

        // Callback tới nghĩa là giao dịch thành công; app_trans_id nằm trong data JSON.
        success = true;
        try
        {
            using var doc = JsonDocument.Parse(data);
            if (doc.RootElement.TryGetProperty("app_trans_id", out var t))
                transId = t.GetString();
        }
        catch (JsonException) { /* data không phải JSON hợp lệ — vẫn coi mac đã khớp */ }

        return true;
    }
}
