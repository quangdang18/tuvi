using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Tuvi.Api.Auth;
using Tuvi.Api.Data;
using Tuvi.Api.Payments;
using Tuvi.Api.Push;
using Tuvi.Api.Services;
using Tuvi.Api.Time;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON: xuất enum dạng chữ ("Hoa", "MoMo") thay vì số cho dễ đọc.
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Tuvi API",
        Version = "v1",
        Description = "Backend tử vi / thần số học / 16 nhóm tính cách + user, cá nhân hóa, thanh toán, push."
    });
    // Nút Authorize (Bearer token) trong Swagger UI.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Dán token nhận được từ POST /api/users (không cần chữ 'Bearer')."
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = new List<string>()
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Database: SQLite file, chạy local không cần cài gì.
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=tuvi.db"));

// Cấu hình.
builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection("Payment"));
builder.Services.Configure<PushOptions>(builder.Configuration.GetSection("Push"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Auth (JWT bearer).
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddSingleton<TokenService>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
    });
builder.Services.AddAuthorization();

// Rate limit đăng ký (chống spam tạo user rác).
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddFixedWindowLimiter("register", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddCors(o => o.AddPolicy("all", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Nguồn thời gian chuẩn (giờ VN).
builder.Services.AddSingleton<IClock, SystemClock>();

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
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
