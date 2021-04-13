using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var performanceCount = await db.Performances.CountAsync(cancellationToken);
            var meetCount = await db.Meets.CountAsync(cancellationToken);
            var seasonCount = await db.Seasons.CountAsync(cancellationToken);
            var athleteCount = await db.Athletes.CountAsync(cancellationToken);

            var viewModel = new HomeViewModel(performanceCount, meetCount, seasonCount, athleteCount, DateTime.Now);
            return View(viewModel);
        }

        [Route("leaderboard")]
        public async Task<IActionResult> Leaderboard(CancellationToken cancellationToken)
        {
            var boysEvents = await db.TrackEvents.Where(t => !t.Gender && t.Performances.Count > 0).OrderBy(t => t.SortOrder).ToListAsync(cancellationToken);
            var girlsEvents = await db.TrackEvents.Where(t => t.Gender && t.Performances.Count > 0).OrderBy(t => t.SortOrder).ToListAsync(cancellationToken);

            var boysEventsWithTopPerformance = new Dictionary<TrackEvent, KeyValuePair<Performance, List<Athlete>>>();
            var girlsEventsWithTopPerformance = new Dictionary<TrackEvent, KeyValuePair<Performance, List<Athlete>>>();

            KeyValuePair<Performance, List<Athlete>> performanceDictionary;
            foreach (var ev in boysEvents)
            {
                var performances = await db.Performances
                    .Include(p => p.Athlete)
                    .Include(p => p.Meet)
                    .Where(p => p.TrackEventId == ev.Id)
                    .ToListAsync(cancellationToken);

                var performance = ev.RunningEvent ? performances.MinBy(p => p.TotalSeconds).FirstOrDefault() : performances.MaxBy(p => p.TotalInches).FirstOrDefault();

                if (ev.RelayEvent)
                {
                    var matching = await db.Performances
                        .Include(p => p.Athlete)
                        .Include(p => p.Meet)
                        .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Minutes == performance.Minutes && p.Seconds == performance.Seconds && p.Milliseconds == performance.Milliseconds)
                        .Select(p => p.Athlete)
                        .ToListAsync(cancellationToken);

                    performanceDictionary = new KeyValuePair<Performance, List<Athlete>>(performance, matching);
                }
                else
                {
                    performanceDictionary = new KeyValuePair<Performance, List<Athlete>>(performance, new List<Athlete> { performance.Athlete });
                }

                boysEventsWithTopPerformance.Add(ev, performanceDictionary);
            }

            foreach (var ev in girlsEvents)
            {
                var performances = await db.Performances
                    .Include(p => p.Athlete)
                    .Include(p => p.Meet)
                    .Where(p => p.TrackEventId == ev.Id)
                    .ToListAsync(cancellationToken);

                var performance = ev.RunningEvent ? performances.MinBy(p => p.TotalSeconds).FirstOrDefault() : performances.MaxBy(p => p.TotalInches).FirstOrDefault();

                if (ev.RelayEvent)
                {
                    var matching = await db.Performances
                        .Include(p => p.Athlete)
                        .Include(p => p.Meet)
                        .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Minutes == performance.Minutes && p.Seconds == performance.Seconds && p.Milliseconds == performance.Milliseconds)
                        .Select(p => p.Athlete)
                        .ToListAsync(cancellationToken);

                    performanceDictionary = new KeyValuePair<Performance, List<Athlete>>(performance, matching);
                }
                else
                {
                    performanceDictionary = new KeyValuePair<Performance, List<Athlete>>(performance, new List<Athlete> { performance.Athlete });
                }

                girlsEventsWithTopPerformance.Add(ev, performanceDictionary);
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

            var performancesDictionary = new Dictionary<Performance, List<Athlete>>();
            performances = selectedEvent.RunningEvent ? performances.OrderBy(p => p.TotalSeconds).ToList() : performances.OrderByDescending(p => p.TotalInches).ToList();
            if (selectedEvent.RelayEvent)
            {
                if (selectedEvent.RunningEvent)
                {
                    performances = performances.DistinctBy(p => p.TotalSeconds).ToList();

                    foreach (var performance in performances)
                    {
                        var matching = await db.Performances
                            .Include(p => p.Athlete)
                            .Include(p => p.Meet)
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Minutes == performance.Minutes && p.Seconds == performance.Seconds && p.Milliseconds == performance.Milliseconds)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        performancesDictionary.Add(performance, matching);
                    }
                }
                else
                {
                    performances = performances.DistinctBy(p => p.TotalInches).ToList();

                    foreach (var performance in performances)
                    {
                        var matching = await db.Performances
                            .Include(p => p.Athlete)
                            .Include(p => p.Meet)
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Feet == performance.Feet && p.Inches == performance.Inches && p.FractionalInches == performance.FractionalInches)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        performancesDictionary.Add(performance, matching);
                    }
                }
            }
            else
            {
                foreach (var performance in performances)
                {
                    performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                }
            }

            var viewModel = new EventLeaderboardViewModel(selectedEvent, performancesDictionary);

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

            var lifetimePrs = GetLifetimePrs(athlete);
            var seasonPrs = GetSeasonPrs(athlete);
            var eventPerformances = GetEventPerformances(athlete);

            return View(new AthleteViewModel(athlete, lifetimePrs, seasonPrs, eventPerformances));
        }

        private List<LifetimePr> GetLifetimePrs(Athlete athlete)
        {
            var lifetimePrs = new List<LifetimePr>();
            var groupedPerformances = athlete.Performances.GroupBy(p => p.TrackEvent).OrderBy(g => g.Key.SortOrder);
            foreach (var group in groupedPerformances)
            {
                Performance outdoorLifetimePr = null;
                Performance indoorLifetimePr = null;

                if (group.Any(p => p.Meet.Outdoor))
                {
                    outdoorLifetimePr = group.Key.RunningEvent ? group.Where(p => p.Meet.Outdoor).MinBy(p => p.TotalSeconds).First() : group.Where(p => p.Meet.Outdoor).MaxBy(p => p.TotalInches).First();
                }

                if (group.Any(p => !p.Meet.Outdoor))
                {
                    indoorLifetimePr = group.Key.RunningEvent ? group.Where(p => !p.Meet.Outdoor).MinBy(p => p.TotalSeconds).First() : group.Where(p => !p.Meet.Outdoor).MaxBy(p => p.TotalInches).First();
                }

                var lifetimePr = new LifetimePr(group.Key, outdoorLifetimePr, indoorLifetimePr);
                lifetimePrs.Add(lifetimePr);
            }

            return lifetimePrs;
        }

        private List<SeasonPr> GetSeasonPrs(Athlete athlete)
        {
            var seasonPrs = new List<SeasonPr>();

            var performancesGroupedBySeason = athlete.Performances.GroupBy(p => p.Meet.Season).OrderByDescending(g => g.Key.Name);
            foreach (var group in performancesGroupedBySeason)
            {
                var eventPrs = new List<EventPr>();

                var seasonPerformancesGroupedByEvent = group.GroupBy(p => p.TrackEvent).OrderBy(p => p.Key.SortOrder);
                foreach (var eventGroup in seasonPerformancesGroupedByEvent)
                {
                    Performance outdoorSeasonPr = null;
                    Performance indoorSeasonPr = null;

                    if (eventGroup.Any(p => p.Meet.Outdoor))
                    {
                        outdoorSeasonPr = eventGroup.Key.RunningEvent ? eventGroup.Where(p => p.Meet.Outdoor).MinBy(p => p.TotalSeconds).FirstOrDefault() : eventGroup.Where(p => p.Meet.Outdoor).MaxBy(p => p.TotalInches).FirstOrDefault();
                    }

                    if (eventGroup.Any(p => !p.Meet.Outdoor))
                    {
                        indoorSeasonPr = eventGroup.Key.RunningEvent ? eventGroup.Where(p => !p.Meet.Outdoor).MinBy(p => p.TotalSeconds).FirstOrDefault() : eventGroup.Where(p => !p.Meet.Outdoor).MaxBy(p => p.TotalInches).FirstOrDefault();
                    }

                    var eventPr = new EventPr(eventGroup.Key, outdoorSeasonPr, indoorSeasonPr);
                    eventPrs.Add(eventPr);
                }

                var seasonPr = new SeasonPr(group.Key, eventPrs);
                seasonPrs.Add(seasonPr);
            }

            return seasonPrs;
        }

        private List<EventPerformance> GetEventPerformances(Athlete athlete)
        {
            var eventPerformances = new List<EventPerformance>();

            var performancesGroupedByEvent = athlete.Performances.GroupBy(p => p.TrackEvent).OrderBy(p => p.Key.SortOrder);
            foreach (var eventGroup in performancesGroupedByEvent)
            {
                IOrderedEnumerable<Performance> performances;
                if (eventGroup.Key.RunningEvent)
                {
                    performances = eventGroup.OrderBy(p => p.TotalSeconds);
                }
                else
                {
                    performances = eventGroup.OrderByDescending(p => p.TotalInches);
                }

                var eventPerformance = new EventPerformance(eventGroup.Key, performances);
                eventPerformances.Add(eventPerformance);
            }

            return eventPerformances;
        }

        [Route("meets")]
        public async Task<IActionResult> Meets()
        {
            var seasons = await db.Seasons
                .Include(s => s.Meets)
                .ThenInclude(m => m.Performances)
                .OrderByDescending(s => s.Name)
                .ToListAsync();

            var objectModel = new List<Tuple<Season, List<Meet>, List<Meet>>>();
            foreach (var season in seasons)
            {
                var indoorMeets = season.Meets.Where(m => !m.Outdoor).ToList();
                var outdoorMeets = season.Meets.Where(m => m.Outdoor).ToList();

                objectModel.Add(new Tuple<Season, List<Meet>, List<Meet>>(season, indoorMeets, outdoorMeets));
            }

            var viewModel = new MeetsViewModel(objectModel);
            return View(viewModel);
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

            var boysResultsDictionary = new Dictionary<TrackEvent, Dictionary<Performance, List<Athlete>>>();
            foreach (var res in boysResults)
            {
                var performancesDictionary = new Dictionary<Performance, List<Athlete>>();

                if (res.Key.RelayEvent)
                {
                    var performances = res.Value.DistinctBy(p => p.TotalSeconds);

                    foreach (var performance in performances)
                    {
                        var matching = await db.Performances
                            .Include(p => p.Athlete)
                            .Include(p => p.Meet)
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Minutes == performance.Minutes && p.Seconds == performance.Seconds && p.Milliseconds == performance.Milliseconds)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        performancesDictionary.Add(performance, matching);
                    }
                }
                else
                {
                    foreach (var performance in res.Value)
                    {
                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                }

                boysResultsDictionary.Add(res.Key, performancesDictionary);
            }

            var girlsResults = girlsPerformancesGroupedByEvent.ToDictionary(group => group.Key, group => group.Key.RunningEvent
                ? group.OrderBy(p => p.TotalSeconds).ToList()
                : group.OrderByDescending(p => p.TotalInches).ToList());

            var girlsResultsDictionary = new Dictionary<TrackEvent, Dictionary<Performance, List<Athlete>>>();
            foreach (var res in girlsResults)
            {
                var performancesDictionary = new Dictionary<Performance, List<Athlete>>();

                if (res.Key.RelayEvent)
                {
                    var performances = res.Value.DistinctBy(p => p.TotalSeconds);

                    foreach (var performance in performances)
                    {
                        var matching = await db.Performances
                            .Include(p => p.Athlete)
                            .Include(p => p.Meet)
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId && p.Minutes == performance.Minutes && p.Seconds == performance.Seconds && p.Milliseconds == performance.Milliseconds)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        performancesDictionary.Add(performance, matching);
                    }
                }
                else
                {
                    foreach (var performance in res.Value)
                    {
                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                }

                girlsResultsDictionary.Add(res.Key, performancesDictionary);
            }

            var viewModel = new MeetDetailsViewModel(selectedMeet, boysResultsDictionary, girlsResultsDictionary);
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
