using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tuvi.Api.Auth;

/// <summary>Sinh JWT cho user (tài khoản theo thiết bị, không cần mật khẩu ở MVP).</summary>
public class TokenService
{
    private readonly JwtOptions _opt;

    public TokenService(IOptions<JwtOptions> opt) => _opt = opt.Value;

    public string Create(int userId, string displayName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("name", displayName),
        };
        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_opt.ExpireDays),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
