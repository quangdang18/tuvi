using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoroscopeController : ControllerBase
{
    private readonly HoroscopeService _horoscope;
    private readonly ZodiacService _zodiac;

    public HoroscopeController(HoroscopeService horoscope, ZodiacService zodiac)
    {
        _horoscope = horoscope;
        _zodiac = zodiac;
    }

    /// <summary>Danh sách 12 cung hoàng đạo.</summary>
    [HttpGet("signs")]
    public ActionResult<IEnumerable<ZodiacInfo>> Signs() => Ok(_zodiac.GetAll());

    /// <summary>Tử vi hằng ngày theo cung (vd: leo). Bỏ trống date = hôm nay.</summary>
    [HttpGet("daily/{sign}")]
    public ActionResult<DailyHoroscope> Daily(string sign, [FromQuery] DateOnly? date)
    {
        var result = _horoscope.GetDaily(sign, date ?? Today());
        return result is null ? NotFound($"Không tìm thấy cung '{sign}'.") : result;
    }

    /// <summary>Tử vi hằng ngày theo ngày sinh (tự suy ra cung).</summary>
    [HttpGet("daily-by-birth")]
    public ActionResult<DailyHoroscope> DailyByBirth([FromQuery] DateOnly birthDate, [FromQuery] DateOnly? date)
    {
        var sign = _zodiac.GetByDate(birthDate);
        return _horoscope.GetDaily(sign.Key, date ?? Today())!;
    }

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);
}
