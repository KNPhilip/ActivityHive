using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[AllowAnonymous]
public sealed class FallbackController : Controller
{
    public IActionResult Index() =>
        PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
}
