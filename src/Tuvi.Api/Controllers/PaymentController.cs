using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Payments;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ApiControllerBase
{
    private readonly PaymentService _payment;

    public PaymentController(PaymentService payment) => _payment = payment;

    /// <summary>Tạo đơn nâng cấp Premium cho user hiện tại → trả link thanh toán (Mock: trang mô phỏng local).</summary>
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<PaymentCreateResult>> Create([FromBody] PaymentCreateRequest req)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        var res = await _payment.CreateAsync(CurrentUserId, req, baseUrl);
        return res is null ? NotFound("User hoặc cổng thanh toán không hợp lệ.") : res;
    }

    /// <summary>Tra trạng thái đơn.</summary>
    [HttpGet("status")]
    public async Task<ActionResult<PaymentStatusResult>> Status([FromQuery] string orderId)
    {
        var res = await _payment.GetStatusAsync(orderId);
        return res is null ? NotFound() : res;
    }

    /// <summary>[Mock] Mô phỏng thanh toán thành công để test local (chặn ở Live mode).</summary>
    [HttpPost("simulate-success")]
    public async Task<ActionResult<PaymentStatusResult>> Simulate([FromBody] SimulateRequest req)
    {
        if (!_payment.IsMock) return BadRequest("Chỉ dùng được ở Mock mode.");
        var res = await _payment.MarkPaidAsync(req.OrderId, "MOCK-" + req.OrderId);
        return res is null ? NotFound() : res;
    }

    /// <summary>IPN thật từ MoMo (chỉ chạy ở Live mode).</summary>
    [HttpPost("momo/ipn")]
    public async Task<IActionResult> MoMoIpn()
    {
        var fields = await ReadFieldsAsync();
        var provider = _payment.GetProvider(PaymentProviderKind.MoMo);
        if (provider is null || !provider.VerifyCallback(fields, out bool ok, out var transId))
            return BadRequest(new { resultCode = 1, message = "Invalid signature" });
        if (ok) await _payment.MarkPaidAsync(Get(fields, "orderId"), transId);
        return Ok(new { resultCode = 0, message = "Success" });
    }

    /// <summary>Callback thật từ ZaloPay (chỉ chạy ở Live mode).</summary>
    [HttpPost("zalopay/callback")]
    public async Task<IActionResult> ZaloPayCallback()
    {
        var fields = await ReadFieldsAsync();
        var provider = _payment.GetProvider(PaymentProviderKind.ZaloPay);
        if (provider is null || !provider.VerifyCallback(fields, out bool ok, out var transId))
            return Ok(new { return_code = -1, return_message = "mac not equal" });
        if (ok && transId is not null) await _payment.MarkPaidAsync(transId, transId);
        return Ok(new { return_code = 1, return_message = "success" });
    }

    private async Task<Dictionary<string, string>> ReadFieldsAsync()
    {
        if (Request.HasFormContentType)
            return Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());

        using var reader = new StreamReader(Request.Body);
        string body = await reader.ReadToEndAsync();
        if (string.IsNullOrWhiteSpace(body)) return new();
        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);
            return dict?.ToDictionary(k => k.Key, v => v.Value.ToString()) ?? new();
        }
        catch (JsonException) { return new(); }
    }

    private static string Get(Dictionary<string, string> f, string k) => f.TryGetValue(k, out var v) ? v : "";
}
