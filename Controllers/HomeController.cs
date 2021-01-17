using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MoreLinq;

namespace CloverleafTrack.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly CloverleafTrackDataContext db;
        private readonly CurrentSeasonOptions currentSeason;

        public HomeController(CloverleafTrackDataContext db, IOptions<CurrentSeasonOptions> currentSeason)
        {
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
            var boysEvents = await db.TrackEvents.Where(t => !t.Gender).OrderBy(t => t.SortOrder).ToListAsync();
            var girlsEvents = await db.TrackEvents.Where(t => t.Gender).OrderBy(t => t.SortOrder).ToListAsync();

            var boysEventsWithTopPerformance = new Dictionary<TrackEvent, Performance>();
            var girlsEventsWithTopPerformance = new Dictionary<TrackEvent, Performance>();

            foreach (var ev in boysEvents)
            {
                var performances = await db.Performances
                    .Include(p => p.Athlete)
                    .Include(p => p.Meet)
                    .Where(p => p.TrackEventId == ev.Id)
                    .ToListAsync();

                Performance performance;
                if (ev.RunningEvent)
                {
                    performance = performances.MinBy(p => p.TotalSeconds).FirstOrDefault();
                }
                else
                {
                    performance = performances.MaxBy(p => p.TotalInches).FirstOrDefault();
                }

                boysEventsWithTopPerformance.Add(ev, performance);
            }

            foreach (var ev in girlsEvents)
            {
                var performances = await db.Performances
                    .Include(p => p.Athlete)
                    .Include(p => p.Meet)
                    .Where(p => p.TrackEventId == ev.Id)
                    .ToListAsync();

                Performance performance;
                if (ev.RunningEvent)
                {
                    performance = performances.MinBy(p => p.TotalSeconds).FirstOrDefault();
                }
                else
                {
                    performance = performances.MaxBy(p => p.TotalInches).FirstOrDefault();
                }

                girlsEventsWithTopPerformance.Add(ev, performance);
            }

            var viewModel = new LeaderboardViewModel(boysEventsWithTopPerformance, girlsEventsWithTopPerformance);
            return View(viewModel);
        }

        [Route("leaderboard/{eventName}")]
        public async Task<IActionResult> EventLeaderboard(string eventName)
        {
            var trackEvents = await db.TrackEvents.ToListAsync();
            var selectedEvent = trackEvents.FirstOrDefault(t => t.UrlName == eventName);
            if (selectedEvent == null)
            {
                return NotFound();
            }

            var performances = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Where(p => p.TrackEventId == selectedEvent.Id)
                .ToListAsync();

            performances = selectedEvent.RunningEvent ? performances.OrderBy(p => p.TotalSeconds).ToList() : performances.OrderByDescending(p => p.TotalInches).ToList();

            var viewModel = new EventLeaderboardViewModel(selectedEvent, performances);

            return View("EventLeaderboard", viewModel);
        }

        [Route("roster")]
        public async Task<IActionResult> Roster()
        {
            var currentAthletes = await db.Athletes
                .Where(a => a.GraduationYear >= currentSeason.GraduationYear && a.GraduationYear <= (currentSeason.GraduationYear + 3))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToListAsync();

            var graduatedAthletes = await db.Athletes
                .Where(a => a.GraduationYear < currentSeason.GraduationYear)
                .OrderByDescending(a => a.GraduationYear)
                .ThenBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToListAsync();

            return View(new RosterViewModel(currentAthletes, graduatedAthletes));
        }

        [Route("roster/{name}")]
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
                .Include(a => a.Performances)
                .ThenInclude(p => p.TrackEvent)
                .Include(a => a.Performances)
                .ThenInclude(p => p.Meet)
                .ThenInclude(m => m.Season)
                .FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);

            if (athlete == null)
            {
                return NotFound();
            }

            var lifetimePrs = new Dictionary<TrackEvent, Performance>();
            var groupedPerformances = athlete.Performances.GroupBy(p => p.TrackEvent).OrderBy(g => g.Key.SortOrder);
            foreach (var group in groupedPerformances)
            {
                var best = group.Key.RunningEvent ? group.MinBy(p => p.TotalSeconds).First() : group.MaxBy(p => p.TotalInches).First();
                lifetimePrs.Add(group.Key, best);
            }

            var seasonPrs = new Dictionary<Season, Dictionary<TrackEvent, Performance>>();
            var performancesGroupedBySeason = athlete.Performances.GroupBy(p => p.Meet.Season).OrderByDescending(g => g.Key.Name);
            foreach (var group in performancesGroupedBySeason)
            {
                var seasonBestPerformances = new Dictionary<TrackEvent, Performance>();
                var seasonPerformancesGroupedByTrackEvent = group.GroupBy(p => p.TrackEvent).OrderBy(g => g.Key.SortOrder);
                foreach (var secondGroup in seasonPerformancesGroupedByTrackEvent)
                {
                    var best = secondGroup.Key.RunningEvent ? secondGroup.MinBy(p => p.TotalSeconds).First() : secondGroup.MaxBy(p => p.TotalInches).First();
                    seasonBestPerformances.Add(secondGroup.Key, best);
                }

                seasonPrs.Add(group.Key, seasonBestPerformances);
            }

            var performancesGroupedByEvent = athlete.Performances.GroupBy(p => p.TrackEvent).OrderBy(g => g.Key.SortOrder);
            var eventPerformances = performancesGroupedByEvent.ToDictionary(eventGrouping => eventGrouping.Key, eventGrouping => eventGrouping.Key.RunningEvent
                ? eventGrouping.OrderBy(x => x.TotalSeconds)
                : eventGrouping.OrderByDescending(p => p.TotalInches));

            return View(new AthleteViewModel(athlete, lifetimePrs, seasonPrs, eventPerformances));
        }

        [Route("meets")]
        public IActionResult Meets()
        {
            return View();
        }

        [Route("meets/{meetName}")]
        public async Task<IActionResult> MeetResults(string meetName)
        {
            var meets = await db.Meets.ToListAsync();
            var selectedMeet = meets.FirstOrDefault(m => m.UrlName == meetName);
            if (selectedMeet == null)
            {
                return NotFound();
            }

            var boysPerformances = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.TrackEvent)
                .Where(p => p.MeetId == selectedMeet.Id && !p.TrackEvent.Gender)
                .OrderBy(p => p.TrackEvent.SortOrder)
                .ToListAsync();

            var girlsPerformances = await db.Performances
                .Include(p => p.Athlete)
                .Include(p => p.TrackEvent)
                .Where(p => p.MeetId == selectedMeet.Id && p.TrackEvent.Gender)
                .OrderBy(p => p.TrackEvent.SortOrder)
                .ToListAsync();

            var boysPerformancesGroupedByEvent = boysPerformances.GroupBy(p => p.TrackEvent);
            var girlsPerformancesGroupedByEvent = girlsPerformances.GroupBy(p => p.TrackEvent);

            var boysResults = boysPerformancesGroupedByEvent.ToDictionary(group => group.Key, group => group.Key.RunningEvent
                ? group.OrderBy(p => p.TotalSeconds).ToList()
                : group.OrderByDescending(p => p.TotalInches).ToList());

            var girlsResults = girlsPerformancesGroupedByEvent.ToDictionary(group => group.Key, group => group.Key.RunningEvent
                ? group.OrderBy(p => p.TotalSeconds).ToList()
                : group.OrderByDescending(p => p.TotalInches).ToList());

            var viewModel = new MeetDetailsViewModel(selectedMeet, boysResults, girlsResults);
            return View("MeetDetails", viewModel);
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
