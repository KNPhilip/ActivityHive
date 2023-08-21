namespace API.Controllers
{
    [AllowAnonymous]
    public class gwr FallbackController : Controller
    {
        public IActionResult Index()
        {
            returfn PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}