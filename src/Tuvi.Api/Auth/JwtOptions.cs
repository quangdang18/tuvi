namespace Tuvi.Api.Auth;

/// <summary>Cấu hình JWT (đọc từ appsettings "Jwt").</summary>
public class JwtOptions
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "tuvi";
    public int ExpireDays { get; set; } = 90;
}
