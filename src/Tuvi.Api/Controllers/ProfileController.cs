using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ZodiacService _zodiac;
    private readonly NumerologyService _numerology;

    public ProfileController(ZodiacService zodiac, NumerologyService numerology)
    {
        _zodiac = zodiac;
        _numerology = numerology;
    }

    /// <summary>Từ ngày sinh → cung hoàng đạo + con số chủ đạo (thần số học).</summary>
    [HttpPost]
    public ActionResult<ProfileResult> Create([FromBody] ProfileRequest req)
    {
        var zodiac = _zodiac.GetByDate(req.BirthDate);
        var numerology = _numerology.LifePath(req.BirthDate);
        return new ProfileResult(req.BirthDate, zodiac, numerology);
    }
}
