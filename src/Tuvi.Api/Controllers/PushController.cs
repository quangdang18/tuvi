using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Push;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PushController : ControllerBase
{
    private readonly DailyHoroscopePushJob _job;
    private readonly PushLog _log;
    private readonly IWebHostEnvironment _env;

    public PushController(DailyHoroscopePushJob job, PushLog log, IWebHostEnvironment env)
    {
        _job = job;
        _log = log;
        _env = env;
    }

    /// <summary>[Dev] Gửi push tử vi ngay cho mọi user có device token (chỉ Development).</summary>
    [HttpPost("run-now")]
    public async Task<IActionResult> RunNow()
    {
        if (!_env.IsDevelopment()) return NotFound();
        int sent = await _job.SendDailyAsync();
        return Ok(new { sent });
    }

    /// <summary>[Dev] Xem các push gần đây đã "gửi" (chỉ Development).</summary>
    [HttpGet("log")]
    public ActionResult<IEnumerable<PushRecord>> Log()
    {
        if (!_env.IsDevelopment()) return NotFound();
        return Ok(_log.Recent());
    }
}
