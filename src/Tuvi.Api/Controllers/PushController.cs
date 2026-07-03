using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Push;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PushController : ControllerBase
{
    private readonly DailyHoroscopePushJob _job;
    private readonly PushLog _log;

    public PushController(DailyHoroscopePushJob job, PushLog log)
    {
        _job = job;
        _log = log;
    }

    /// <summary>[Dev] Gửi push tử vi ngay cho mọi user có device token (không chờ tới sáng).</summary>
    [HttpPost("run-now")]
    public async Task<IActionResult> RunNow()
    {
        int sent = await _job.SendDailyAsync();
        return Ok(new { sent });
    }

    /// <summary>[Dev] Xem các push gần đây đã "gửi" (Log mode).</summary>
    [HttpGet("log")]
    public ActionResult<IEnumerable<PushRecord>> Log() => Ok(_log.Recent());
}
