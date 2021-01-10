using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MoreLinq;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CloverleafTrack.Controllers
{
    [Route("")]
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

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("leaderboard")]
        public async Task<IActionResult> Leaderboard()
        {
            return View();
        }

        [Route("roster")]
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

        [Route("athlete/{name}")]
        public async Task<IActionResult> Athlete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return NotFound();
            }

            var splitName = name.Split('-');
            var firstName = splitName[0];
            var lastName = splitName[1];

            var athlete = await db.Athletes
                .Include(athlete => athlete.Performances)
                .ThenInclude(performance => performance.TrackEvent)
                .Include(athlete => athlete.Performances)
                .ThenInclude(performance => performance.Meet)
                .FirstOrDefaultAsync(athlete => athlete.FirstName == firstName && athlete.LastName == lastName);

            if (athlete == null)
            {
                return NotFound();
            }

            var lifetimePrs = new Dictionary<TrackEvent, Performance>();
            var groupedPerformances = athlete.Performances.GroupBy(x => x.TrackEvent).OrderBy(x => x.Key.SortOrder);
            foreach (var group in groupedPerformances)
            {
                Performance best;
                if (group.Key.RunningEvent)
                {
                    best = group.MinBy(x => x.TotalSeconds).First();
                    lifetimePrs.Add(group.Key, best);
                }
                else
                {
                    best = group.MaxBy(x => x.TotalInches).First();
                    lifetimePrs.Add(group.Key, best);
                }
            }

            var performancesGroupedByMeet = athlete.Performances.GroupBy(x => x.Meet).OrderByDescending(x => x.Key.Date);
            var meetResults = performancesGroupedByMeet.ToDictionary(group => group.Key, group => group.OrderBy(x => x.TrackEvent.SortOrder).ToList());

            return View(new AthleteViewModel(athlete, lifetimePrs, meetResults));
        }

        [Route("meets")]
        public IActionResult Meets()
        {
            return View();
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
