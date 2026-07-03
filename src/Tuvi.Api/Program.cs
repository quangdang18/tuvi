using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Tuvi.Api.Data;
using Tuvi.Api.Payments;
using Tuvi.Api.Push;
using Tuvi.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON: xuất enum dạng chữ ("Hoa", "MoMo") thay vì số cho dễ đọc.
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new()
    {
        Title = "Tuvi API",
        Version = "v1",
        Description = "Backend tử vi / thần số học / 16 nhóm tính cách + user, cá nhân hóa, thanh toán, push."
    }));

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Database: SQLite file, chạy local không cần cài gì.
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=tuvi.db"));

// Cấu hình thanh toán & push từ appsettings.
builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection("Payment"));
builder.Services.Configure<PushOptions>(builder.Configuration.GetSection("Push"));

builder.Services.AddCors(o => o.AddPolicy("all", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Engine tính toán — stateless nên singleton.
builder.Services.AddSingleton<ZodiacService>();
builder.Services.AddSingleton<NumerologyService>();
builder.Services.AddSingleton<HoroscopeService>();
builder.Services.AddSingleton<PersonalityService>();
builder.Services.AddSingleton<CompatibilityService>();

// Dịch vụ có trạng thái (dùng DbContext) — scoped.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PaymentService>();

// Cổng thanh toán.
builder.Services.AddSingleton<IPaymentProvider, MoMoPaymentProvider>();
builder.Services.AddSingleton<IPaymentProvider, ZaloPayPaymentProvider>();

// Push: sender + log + job nền chạy mỗi sáng.
builder.Services.AddSingleton<PushLog>();
builder.Services.AddSingleton<IPushSender, LogPushSender>();
builder.Services.AddSingleton<DailyHoroscopePushJob>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DailyHoroscopePushJob>());

var app = builder.Build();

// Tạo database nếu chưa có (MVP: EnsureCreated thay cho migration).
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tuvi API v1"));

// Trang demo tĩnh trong wwwroot (mở http://localhost:5xxx/).
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("all");
app.MapControllers();

app.Run();
