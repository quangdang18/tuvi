using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _users;

    public UsersController(UserService users) => _users = users;

    /// <summary>Đăng ký user mới (lưu ngày/giờ/nơi sinh). Trả về hồ sơ + cung + thần số học.</summary>
    [HttpPost]
    public async Task<ActionResult<UserResult>> Register([FromBody] RegisterUserRequest req)
        => await _users.RegisterAsync(req);

    /// <summary>Lấy hồ sơ user.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResult>> Get(int id)
    {
        var u = await _users.GetAsync(id);
        return u is null ? NotFound() : u;
    }

    /// <summary>Đăng ký device token để nhận push buổi sáng.</summary>
    [HttpPost("{id:int}/device-token")]
    public async Task<IActionResult> DeviceToken(int id, [FromBody] DeviceTokenRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Token)) return BadRequest("Token trống.");
        return await _users.SetDeviceTokenAsync(id, req.Token) ? NoContent() : NotFound();
    }

    /// <summary>Check-in tâm trạng hôm nay → trả về tử vi cá nhân hóa + streak.</summary>
    [HttpPost("{id:int}/checkin")]
    public async Task<ActionResult<CheckinResponse>> Checkin(int id, [FromBody] CheckinRequest req)
    {
        var res = await _users.CheckinAsync(id, req, Today());
        return res is null ? NotFound() : new CheckinResponse(res.Value.Horoscope, res.Value.Streak);
    }

    /// <summary>Tử vi cá nhân hóa (dùng mood đã check-in trong ngày nếu có).</summary>
    [HttpGet("{id:int}/horoscope")]
    public async Task<ActionResult<PersonalizedHoroscope>> Horoscope(int id, [FromQuery] DateOnly? date)
    {
        var res = await _users.GetPersonalizedAsync(id, date ?? Today());
        return res is null ? NotFound() : res;
    }

    /// <summary>Chuỗi check-in (streak) hiện tại và dài nhất.</summary>
    [HttpGet("{id:int}/streak")]
    public async Task<ActionResult<StreakResult>> Streak(int id)
    {
        var res = await _users.GetStreakAsync(id, Today());
        return res is null ? NotFound() : res;
    }

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);
}
