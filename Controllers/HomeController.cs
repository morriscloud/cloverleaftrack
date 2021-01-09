using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly CloverleafTrackDataContext db;
        private readonly CurrentSeasonOptions currentSeason;

        public HomeController(ILogger<HomeController> logger, CloverleafTrackDataContext db, IOptions<CurrentSeasonOptions> currentSeason)
        {
            this.logger = logger;
            this.db = db;
            this.currentSeason = currentSeason.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Leaderboard()
        {
            return View();
        }

        public async Task<IActionResult> Roster()
        {
            var currentAthletes = await db.Athletes
                .Where(x => x.GraduationYear >= currentSeason.GraduationYear && x.GraduationYear <= (currentSeason.GraduationYear + 3))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();

            var graduatedAthletes = await db.Athletes
                .Where(x => x.GraduationYear < currentSeason.GraduationYear)
                .OrderByDescending(x => x.GraduationYear)
                .ThenBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();

            return View(new RosterViewModel(currentAthletes, graduatedAthletes));
        }

        public IActionResult Meets()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
