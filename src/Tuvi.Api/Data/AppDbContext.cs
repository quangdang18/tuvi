using Microsoft.EntityFrameworkCore;

namespace Tuvi.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<DailyCheckin> Checkins => Set<DailyCheckin>();
    public DbSet<PaymentOrder> PaymentOrders => Set<PaymentOrder>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>()
            .HasMany(u => u.Checkins)
            .WithOne(c => c.User!)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Mỗi user chỉ check-in 1 lần/ngày.
        b.Entity<DailyCheckin>()
            .HasIndex(c => new { c.UserId, c.Date })
            .IsUnique();

        b.Entity<PaymentOrder>()
            .HasIndex(p => p.OrderId)
            .IsUnique();

        b.Entity<User>()
            .HasIndex(u => u.ReferralCode)
            .IsUnique();
    }
}
