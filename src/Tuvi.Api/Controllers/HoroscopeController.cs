using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;
using Tuvi.Api.Time;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoroscopeController : ControllerBase
{
    private readonly HoroscopeService _horoscope;
    private readonly ZodiacService _zodiac;
    private readonly IClock _clock;

    public HoroscopeController(HoroscopeService horoscope, ZodiacService zodiac, IClock clock)
    {
        _horoscope = horoscope;
        _zodiac = zodiac;
        _clock = clock;
    }

    /// <summary>Danh sách 12 cung hoàng đạo.</summary>
    [HttpGet("signs")]
    public ActionResult<IEnumerable<ZodiacInfo>> Signs() => Ok(_zodiac.GetAll());

    /// <summary>Tử vi hằng ngày theo cung (vd: leo). Bỏ trống date = hôm nay.</summary>
    [HttpGet("daily/{sign}")]
    public ActionResult<DailyHoroscope> Daily(string sign, [FromQuery] DateOnly? date)
    {
        var result = _horoscope.GetDaily(sign, date ?? _clock.Today);
        return result is null ? NotFound($"Không tìm thấy cung '{sign}'.") : result;
    }

    /// <summary>Tử vi hằng ngày theo ngày sinh (tự suy ra cung).</summary>
    [HttpGet("daily-by-birth")]
    public ActionResult<DailyHoroscope> DailyByBirth([FromQuery] DateOnly birthDate, [FromQuery] DateOnly? date)
    {
        var sign = _zodiac.GetByDate(birthDate);
        return _horoscope.GetDaily(sign.Key, date ?? _clock.Today)!;
    }

    /// <summary>Tử vi tuần theo cung. Bỏ trống date = tuần này (chứa ngày đó).</summary>
    [HttpGet("weekly/{sign}")]
    public ActionResult<WeeklyHoroscope> Weekly(string sign, [FromQuery] DateOnly? date)
    {
        var result = _horoscope.GetWeekly(sign, date ?? _clock.Today);
        return result is null ? NotFound($"Không tìm thấy cung '{sign}'.") : result;
    }

    /// <summary>Tử vi tháng theo cung. Bỏ trống year/month = tháng này.</summary>
    [HttpGet("monthly/{sign}")]
    public ActionResult<MonthlyHoroscope> Monthly(string sign, [FromQuery] int? year, [FromQuery] int? month)
    {
        var today = _clock.Today;
        var result = _horoscope.GetMonthly(sign, year ?? today.Year, month ?? today.Month);
        return result is null ? NotFound($"Cung '{sign}' hoặc tháng không hợp lệ.") : result;
    }
}
