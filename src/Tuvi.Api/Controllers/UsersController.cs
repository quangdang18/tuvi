using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tuvi.Api.Auth;
using Tuvi.Api.Models;
using Tuvi.Api.Services;
using Tuvi.Api.Time;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly UserService _users;
    private readonly TokenService _tokens;
    private readonly IClock _clock;

    public UsersController(UserService users, TokenService tokens, IClock clock)
    {
        _users = users;
        _tokens = tokens;
        _clock = clock;
    }

    /// <summary>Đăng ký user mới (lưu ngày/giờ/nơi sinh). Trả token JWT + hồ sơ.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("register")]
    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.DisplayName) || req.DisplayName.Trim().Length > 50)
            return BadRequest("Tên hiển thị bắt buộc và tối đa 50 ký tự.");
        if (req.BirthDate > _clock.Today || req.BirthDate < new DateOnly(1900, 1, 1))
            return BadRequest("Ngày sinh không hợp lệ.");
        if (req.BirthPlace is { Length: > 100 })
            return BadRequest("Nơi sinh tối đa 100 ký tự.");

        var user = await _users.RegisterAsync(req);
        string token = _tokens.Create(user.Id, user.DisplayName);
        return new AuthResponse(token, user);
    }

    /// <summary>Hồ sơ user hiện tại.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResult>> Get(int id)
    {
        if (id != CurrentUserId) return Forbid();
        var u = await _users.GetAsync(id);
        return u is null ? NotFound() : u;
    }

    /// <summary>Đăng ký device token để nhận push buổi sáng.</summary>
    [HttpPost("{id:int}/device-token")]
    public async Task<IActionResult> DeviceToken(int id, [FromBody] DeviceTokenRequest req)
    {
        if (id != CurrentUserId) return Forbid();
        if (string.IsNullOrWhiteSpace(req.Token)) return BadRequest("Token trống.");
        return await _users.SetDeviceTokenAsync(id, req.Token) ? NoContent() : NotFound();
    }

    /// <summary>Check-in tâm trạng hôm nay → trả về tử vi cá nhân hóa + streak.</summary>
    [HttpPost("{id:int}/checkin")]
    public async Task<ActionResult<CheckinResponse>> Checkin(int id, [FromBody] CheckinRequest req)
    {
        if (id != CurrentUserId) return Forbid();
        var res = await _users.CheckinAsync(id, req);
        return res is null ? NotFound() : new CheckinResponse(res.Value.Horoscope, res.Value.Streak);
    }

    /// <summary>Tử vi cá nhân hóa (dùng mood đã check-in trong ngày nếu có).</summary>
    [HttpGet("{id:int}/horoscope")]
    public async Task<ActionResult<PersonalizedHoroscope>> Horoscope(int id, [FromQuery] DateOnly? date)
    {
        if (id != CurrentUserId) return Forbid();
        var res = await _users.GetPersonalizedAsync(id, date);
        return res is null ? NotFound() : res;
    }

    /// <summary>Chuỗi check-in (streak) hiện tại và dài nhất.</summary>
    [HttpGet("{id:int}/streak")]
    public async Task<ActionResult<StreakResult>> Streak(int id)
    {
        if (id != CurrentUserId) return Forbid();
        var res = await _users.GetStreakAsync(id);
        return res is null ? NotFound() : res;
    }

    /// <summary>Mã &amp; link mời bạn của user + số người đã mời được.</summary>
    [HttpGet("{id:int}/referral")]
    public async Task<ActionResult<ReferralInfo>> Referral(int id)
    {
        if (id != CurrentUserId) return Forbid();
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        var res = await _users.GetReferralAsync(id, baseUrl);
        return res is null ? NotFound() : res;
    }

    /// <summary>Cập nhật mối quan tâm chính (onboarding) để cá nhân hóa điểm nhấn.</summary>
    [HttpPut("{id:int}/focus")]
    public async Task<IActionResult> Focus(int id, [FromBody] FocusRequest req)
    {
        if (id != CurrentUserId) return Forbid();
        return await _users.SetFocusAsync(id, req.Focus) ? NoContent() : NotFound();
    }
}
