using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Tuvi.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Id user từ JWT đã xác thực.</summary>
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
