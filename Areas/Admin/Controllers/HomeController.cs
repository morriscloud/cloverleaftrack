using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloverleafTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
