using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Landing()
        {
            return View();
        }
    }
}
