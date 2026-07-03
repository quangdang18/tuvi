using System.Text.Json.Serialization;
using Tuvi.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON: xuất enum dạng chữ ("Hoa") thay vì số cho dễ đọc.
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new()
    {
        Title = "Tuvi API",
        Version = "v1",
        Description = "Backend tử vi / thần số học / 16 nhóm tính cách — dùng chung cho web và mobile."
    }));

builder.Services.AddMemoryCache();

// CORS mở cho local dev (web + app mobile gọi chung một backend).
builder.Services.AddCors(o => o.AddPolicy("all", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Engine tính toán — stateless nên đăng ký singleton.
builder.Services.AddSingleton<ZodiacService>();
builder.Services.AddSingleton<NumerologyService>();
builder.Services.AddSingleton<HoroscopeService>();
builder.Services.AddSingleton<PersonalityService>();
builder.Services.AddSingleton<CompatibilityService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tuvi API v1"));

// Trang demo tĩnh trong wwwroot (mở http://localhost:5xxx/).
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("all");
app.MapControllers();

app.Run();
