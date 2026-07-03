using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tuvi.Api.Data;
using Tuvi.Api.Models;

namespace Tuvi.Api.Payments;

/// <summary>Cổng MoMo (API v2 create + IPN). Ở Mock mode trả về trang thanh toán mô phỏng.</summary>
public class MoMoPaymentProvider : IPaymentProvider
{
    private readonly PaymentOptions _opt;
    private readonly IHttpClientFactory _http;

    public MoMoPaymentProvider(IOptions<PaymentOptions> opt, IHttpClientFactory http)
    {
        _opt = opt.Value;
        _http = http;
    }

    public PaymentProviderKind Kind => PaymentProviderKind.MoMo;

    public string NewOrderId() => "MOMO" + Guid.NewGuid().ToString("N")[..16];

    public async Task<string> CreatePayUrlAsync(PaymentOrder order, string returnBaseUrl)
    {
        if (!_opt.IsLive)
            return MockPayUrl.Build(returnBaseUrl, order);

        var m = _opt.MoMo;
        string requestId = Guid.NewGuid().ToString("N");
        const string orderInfo = "Nang cap Premium Tu Vi";
        const string requestType = "captureWallet";
        const string extraData = "";
        string redirectUrl = $"{returnBaseUrl}/payment-return.html";
        string ipnUrl = $"{returnBaseUrl}/api/payment/momo/ipn";

        // Chuỗi ký theo đúng thứ tự alphabet mà MoMo yêu cầu.
        string raw =
            $"accessKey={m.AccessKey}&amount={order.Amount}&extraData={extraData}&ipnUrl={ipnUrl}" +
            $"&orderId={order.OrderId}&orderInfo={orderInfo}&partnerCode={m.PartnerCode}" +
            $"&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
        string signature = Crypto.HmacSha256(raw, m.SecretKey);

        var payload = new
        {
            partnerCode = m.PartnerCode,
            accessKey = m.AccessKey,
            requestId,
            amount = order.Amount.ToString(),
            orderId = order.OrderId,
            orderInfo,
            redirectUrl,
            ipnUrl,
            extraData,
            requestType,
            signature,
            lang = "vi"
        };

        var resp = await _http.CreateClient().PostAsJsonAsync(m.Endpoint, payload);
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("payUrl", out var url) ? url.GetString() ?? "" : "";
    }

    public bool VerifyCallback(IReadOnlyDictionary<string, string> f, out bool success, out string? transId)
    {
        success = false;
        transId = null;
        var m = _opt.MoMo;

        string raw =
            $"accessKey={m.AccessKey}&amount={Get(f, "amount")}&extraData={Get(f, "extraData")}" +
            $"&message={Get(f, "message")}&orderId={Get(f, "orderId")}&orderInfo={Get(f, "orderInfo")}" +
            $"&orderType={Get(f, "orderType")}&partnerCode={Get(f, "partnerCode")}&payType={Get(f, "payType")}" +
            $"&requestId={Get(f, "requestId")}&responseTime={Get(f, "responseTime")}&resultCode={Get(f, "resultCode")}" +
            $"&transId={Get(f, "transId")}";
        string expected = Crypto.HmacSha256(raw, m.SecretKey);

        if (!string.Equals(expected, Get(f, "signature"), StringComparison.OrdinalIgnoreCase))
            return false;

        success = Get(f, "resultCode") == "0";
        transId = Get(f, "transId");
        return true;
    }

    private static string Get(IReadOnlyDictionary<string, string> f, string k) =>
        f.TryGetValue(k, out var v) ? v : "";
}
