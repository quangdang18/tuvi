using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tuvi.Api.Data;
using Tuvi.Api.Models;

namespace Tuvi.Api.Payments;

/// <summary>Điều phối tạo đơn, xử lý callback và kích hoạt premium.</summary>
public class PaymentService
{
    private const int PremiumDays = 30;

    private readonly AppDbContext _db;
    private readonly PaymentOptions _opt;
    private readonly IReadOnlyDictionary<PaymentProviderKind, IPaymentProvider> _providers;

    public PaymentService(AppDbContext db, IOptions<PaymentOptions> opt, IEnumerable<IPaymentProvider> providers)
    {
        _db = db;
        _opt = opt.Value;
        _providers = providers.ToDictionary(p => p.Kind);
    }

    public bool IsMock => !_opt.IsLive;

    public IPaymentProvider? GetProvider(PaymentProviderKind kind) =>
        _providers.TryGetValue(kind, out var p) ? p : null;

    public async Task<PaymentCreateResult?> CreateAsync(int userId, PaymentCreateRequest req, string baseUrl)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return null;
        if (!_providers.TryGetValue(req.Provider, out var provider)) return null;

        long amount = _opt.MonthlyPriceVnd;
        var order = new PaymentOrder
        {
            OrderId = provider.NewOrderId(),
            UserId = userId,
            Provider = req.Provider,
            Amount = amount,
            Plan = req.Plan,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        _db.PaymentOrders.Add(order);
        await _db.SaveChangesAsync();

        string payUrl = await provider.CreatePayUrlAsync(order, baseUrl);
        return new PaymentCreateResult(order.OrderId, order.Provider, amount, payUrl, order.Status, _opt.Mode);
    }

    /// <summary>Đánh dấu đơn đã thanh toán và gia hạn premium 30 ngày. Dùng cho cả callback thật lẫn mock.</summary>
    public async Task<PaymentStatusResult?> MarkPaidAsync(string orderId, string? transId)
    {
        var order = await _db.PaymentOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order is null) return null;

        if (order.Status != PaymentStatus.Paid)
        {
            order.Status = PaymentStatus.Paid;
            order.ProviderTransId = transId;
            order.PaidAt = DateTimeOffset.UtcNow;

            var user = await _db.Users.FindAsync(order.UserId);
            if (user is not null)
            {
                // Cộng dồn nếu vẫn còn hạn premium.
                var from = user.PremiumUntil is { } u && u > DateTimeOffset.UtcNow ? u : DateTimeOffset.UtcNow;
                user.PremiumUntil = from.AddDays(PremiumDays);
            }
            await _db.SaveChangesAsync();
        }

        return await GetStatusAsync(order.OrderId);
    }

    public async Task<PaymentStatusResult?> GetStatusAsync(string orderId)
    {
        var order = await _db.PaymentOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order is null) return null;
        var user = await _db.Users.FindAsync(order.UserId);
        return new PaymentStatusResult(order.OrderId, order.Status, order.Amount, order.Provider, user?.PremiumUntil);
    }
}
