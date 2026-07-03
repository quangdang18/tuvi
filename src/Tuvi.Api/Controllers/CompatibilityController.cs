using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompatibilityController : ControllerBase
{
    private readonly CompatibilityService _service;

    public CompatibilityController(CompatibilityService service) => _service = service;

    /// <summary>Độ hợp giữa hai cung. Ví dụ: /api/compatibility?a=leo&amp;b=aries</summary>
    [HttpGet]
    public ActionResult<CompatibilityResult> Get([FromQuery] string a, [FromQuery] string b)
    {
        var result = _service.Evaluate(a, b);
        return result is null
            ? NotFound("Cung không hợp lệ. Dùng key như leo, aries, pisces...")
            : result;
    }
}
