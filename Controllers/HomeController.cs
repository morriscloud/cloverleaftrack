﻿using CloverleafTrack.Data;
using CloverleafTrack.Managers;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CloverleafTrack.Models.TrackEvents;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly CloverleafTrackDataContext db;
        private readonly CurrentSeasonOptions currentSeason;
        private readonly IAthleteManager athleteManager;
        private readonly IMeetManager meetManager;
        private readonly IPerformanceManager performanceManager;
        private readonly ISeasonManager seasonManager;
        private readonly ITrackEventManager trackEventManager;
        private readonly IEventManager<RunningEvent> runningEventManager;
        private readonly IEventManager<RunningRelayEvent> runningRelayEventManager;
        private readonly IEventManager<FieldEvent> fieldEventManager;
        private readonly IEventManager<FieldRelayEvent> fieldRelayEventManager;

        public HomeController(CloverleafTrackDataContext db, IAthleteManager athleteManager, IMeetManager meetManager, IPerformanceManager performanceManager, ISeasonManager seasonManager, ITrackEventManager trackEventManager, IEventManager<RunningEvent> runningEventManager, IEventManager<RunningRelayEvent> runningRelayEventManager, IEventManager<FieldEvent> fieldEventManager, IEventManager<FieldRelayEvent> fieldRelayEventManager, IOptions<CurrentSeasonOptions> currentSeason)
        {
            this.db = db;
            this.athleteManager = athleteManager;
            this.meetManager = meetManager;
            this.performanceManager = performanceManager;
            this.seasonManager = seasonManager;
            this.trackEventManager = trackEventManager;
            this.runningEventManager = runningEventManager;
            this.runningRelayEventManager = runningRelayEventManager;
            this.fieldEventManager = fieldEventManager;
            this.fieldRelayEventManager = fieldRelayEventManager;
            this.currentSeason = currentSeason.Value;
        }

        [Route("reload")]
        public async Task<IActionResult> Reload(CancellationToken cancellationToken)
        {
            await trackEventManager.Reload(cancellationToken);

            foreach (var athlete in await athleteManager.GetAsync(GraduationStatus.Graduated | GraduationStatus.InSchool))
            {
                athleteManager.GetAthletePrs(athlete);
                athleteManager.GetAthleteSeasonBests(athlete);
            }

            performanceManager.GetLeaderboardPerformances(trackEventManager.TrackEvents);

            foreach (var trackEvent in trackEventManager.TrackEvents)
            {
                performanceManager.GetEventLeaderboardPerformances(trackEvent);
                performanceManager.GetEventLeaderboardPrPerformances(trackEvent);
            }

            return RedirectToAction(nameof(Index));
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var performanceCount = await performanceManager.CountAsync();
            var meetCount = await meetManager.CountAsync();
            var seasonCount = await seasonManager.CountAsync();
            var athleteCount = await athleteManager.CountAsync();

            var viewModel = new HomeViewModel(performanceCount, meetCount, seasonCount, athleteCount, DateTime.Now);
            return View(viewModel);
        }

        [Route("leaderboard")]
        public async Task<IActionResult> Leaderboard()
        {
            var boysOutdoorFieldEvents = await fieldEventManager.GetEventsAsync(Gender.Male, Environment.Outdoor);
            var boysIndoorFieldEvents = await fieldEventManager.GetEventsAsync(Gender.Male, Environment.Indoor);
            var girlsOutdoorFieldEvents = await fieldEventManager.GetEventsAsync(Gender.Female, Environment.Outdoor);
            var girlsIndoorFieldEvents = await fieldEventManager.GetEventsAsync(Gender.Female, Environment.Indoor);

            var boysLeaderboardPerformances = performanceManager.GetLeaderboardPerformances(trackEventManager.BoysLeaderboardEvents);
            var girlsLeaderboardPerformances = performanceManager.GetLeaderboardPerformances(trackEventManager.GirlsLeaderboardEvents);

            var viewModel = new LeaderboardViewModel(boysLeaderboardPerformances, girlsLeaderboardPerformances);
            return View(viewModel);
        }

        [Route("leaderboard/{eventName}")]
        public IActionResult EventLeaderboard(string eventName)
        {
            var trackEvents = trackEventManager.TrackEvents;
            var selectedEvent = trackEvents.FirstOrDefault(t => t.UrlName == eventName);
            if (selectedEvent == null)
            {
                return NotFound();
            }

            var eventLeaderboardPerformances = performanceManager.GetEventLeaderboardPerformances(selectedEvent);

            var viewModel = new EventLeaderboardViewModel(selectedEvent, eventLeaderboardPerformances, false);

            return View("EventLeaderboard", viewModel);
        }

        [Route("leaderboard/{eventName}/prs")]
        public IActionResult PrEventLeaderboard(string eventName)
        {
            var trackEvents = trackEventManager.TrackEvents;
            var selectedEvent = trackEvents.FirstOrDefault(t => t.UrlName == eventName);
            if (selectedEvent == null)
            {
                return NotFound();
            }

            var eventLeaderboardPerformances = performanceManager.GetEventLeaderboardPrPerformances(selectedEvent);

            var viewModel = new EventLeaderboardViewModel(selectedEvent, eventLeaderboardPerformances, true);

            return View("EventLeaderboard", viewModel);
        }

        [Route("roster")]
        public async Task<IActionResult> Roster()
        {
            var currentAthletes = await athleteManager.GetAsync(GraduationStatus.InSchool);
            var graduatedAthletes = await athleteManager.GetAsync(GraduationStatus.Graduated);

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
            var firstName = splitName[0].Replace("_", "-");
            var lastName = splitName[1].Replace("_", "-");

            var athlete = await athleteManager.GetByNameAsync(firstName, lastName);

            if (athlete == null)
            {
                return NotFound();
            }

            var lifetimePrs = GetLifetimePrs(athlete);
            var seasonPrs = GetSeasonPrs(athlete);
            var eventPerformances = await GetEventPerformances(athlete);

            return View(new AthleteViewModel(athlete, lifetimePrs, seasonPrs, eventPerformances));
        }

        [Route("meets")]
        public async Task<IActionResult> Meets()
        {
            var seasons = await seasonManager.GetAsync();

            var seasonMeets = new List<SeasonMeets>();
            foreach (var season in seasons)
            {
                var indoorMeets = season.Meets.Where(m => !m.Outdoor).ToList();
                var outdoorMeets = season.Meets.Where(m => m.Outdoor).ToList();
                
                seasonMeets.Add(new SeasonMeets(season, indoorMeets, outdoorMeets));
            }

            var viewModel = new MeetsViewModel(seasonMeets);
            return View(viewModel);
        }

        [Route("meets/{meetName}")]
        public async Task<IActionResult> MeetResults(string meetName)
        {
            var meets = await db.Meets
                .Include(m => m.Season)
                .ToListAsync();
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

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        performancesDictionary.Add(performance, matching);
                    }
                }
                else
                {
                    foreach (var performance in res.Value)
                    {
                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

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

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        performancesDictionary.Add(performance, matching);
                    }
                }
                else
                {
                    foreach (var performance in res.Value)
                    {
                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                }

                girlsResultsDictionary.Add(res.Key, performancesDictionary);
            }

            var viewModel = new MeetDetailsViewModel(selectedMeet, boysResultsDictionary, girlsResultsDictionary);
            return View("MeetDetails", viewModel);
        }

        [Route("seasons")]
        public async Task<IActionResult> Seasons()
        {
            var seasons = await db.Seasons
                .Include(s => s.Meets)
                .Where(s => s.Meets.Any() && s.Meets.Any(m => m.Performances.Any()))
                .ToListAsync();

            var viewModel = new SeasonsViewModel(seasons);
            return View(viewModel);
        }

        [Route("seasons/{seasonName}/details")]
        public async Task<IActionResult> SeasonDetails(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            if (selectedSeason == null)
            {
                return NotFound();
            }

            var outdoorMeets = await db.Meets
                .Include(m => m.Performances)
                .Where(m => m.SeasonId == selectedSeason.Id && m.Outdoor && m.Performances.Any())
                .ToListAsync();

            var indoorMeets = await db.Meets
                .Include(m => m.Performances)
                .Where(m => m.SeasonId == selectedSeason.Id && !m.Outdoor && m.Performances.Any())
                .ToListAsync();

            var outdoorPerformances = await db.Performances
                .Include(p => p.Meet)
                .Include(p => p.Athlete)
                .Include(p => p.TrackEvent)
                .Where(m => m.Meet.SeasonId == selectedSeason.Id && m.Meet.Outdoor)
                .ToListAsync();

            var groupedOutdoor = outdoorPerformances.GroupBy(p => new { p.TrackEvent.Name, p.TrackEvent.RunningEvent });

            var outdoorBests = new List<EventBest>();

            foreach (var outdoor in groupedOutdoor)
            {
                var trackEvents = await db.TrackEvents
                    .Where(t => t.Name == outdoor.Key.Name)
                    .OrderBy(t => t.SortOrder)
                    .ToListAsync();

                var trackEvent = trackEvents.FirstOrDefault();

                var boysBest = outdoor.Key.RunningEvent ? outdoor.Where(p => !p.Athlete.Gender).MinBy(p => p.TotalSeconds) : outdoor.Where(p => !p.Athlete.Gender).MaxBy(p => p.TotalInches);

                var boysAthletes = new List<Athlete>();
                if (boysBest != null)
                {
                    await SetPersonalBest(boysBest);
                    await SetSeasonBest(boysBest);

                    if (trackEvent is { RelayEvent: true })
                    {
                        if (trackEvent.RunningEvent)
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == boysBest.TrackEvent.Name && p.MeetId == boysBest.MeetId && p.Minutes == boysBest.Minutes && p.Seconds == boysBest.Seconds && p.Milliseconds == boysBest.Milliseconds && !p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            boysAthletes = matching;
                        }
                        else
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == boysBest.TrackEvent.Name && p.MeetId == boysBest.MeetId && p.Feet == boysBest.Feet && p.Inches == boysBest.Inches && p.FractionalInches == boysBest.FractionalInches && !p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            boysAthletes = matching;
                        }
                    }
                    else
                    {
                        boysAthletes.Add(boysBest.Athlete);
                    }
                }

                var girlsBest = outdoor.Key.RunningEvent ? outdoor.Where(p => p.Athlete.Gender).MinBy(p => p.TotalSeconds) : outdoor.Where(p => p.Athlete.Gender).MaxBy(p => p.TotalInches);

                var girlsAthletes = new List<Athlete>();
                if (girlsBest != null)
                {
                    await SetPersonalBest(girlsBest);
                    await SetSeasonBest(girlsBest);

                    if (trackEvent is { RelayEvent: true })
                    {
                        if (trackEvent.RunningEvent)
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == girlsBest.TrackEvent.Name && p.MeetId == girlsBest.MeetId && p.Minutes == girlsBest.Minutes && p.Seconds == girlsBest.Seconds && p.Milliseconds == girlsBest.Milliseconds && p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            girlsAthletes = matching;
                        }
                        else
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == girlsBest.TrackEvent.Name && p.MeetId == girlsBest.MeetId && p.Feet == girlsBest.Feet && p.Inches == girlsBest.Inches && p.FractionalInches == girlsBest.FractionalInches && p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            girlsAthletes = matching;
                        }
                    }
                    else
                    {
                        girlsAthletes.Add(girlsBest.Athlete);
                    }
                }

                var eventBest = new EventBest(trackEvent, boysBest, boysAthletes, girlsBest, girlsAthletes);
                outdoorBests.Add(eventBest);
            }

            var indoorPerformances = await db.Performances
                .Include(p => p.Meet)
                .Include(p => p.Athlete)
                .Include(p => p.TrackEvent)
                .Where(m => m.Meet.SeasonId == selectedSeason.Id && !m.Meet.Outdoor)
                .ToListAsync();

            var groupedIndoor = indoorPerformances.GroupBy(p => new { p.TrackEvent.Name, p.TrackEvent.RunningEvent });

            var indoorBests = new List<EventBest>();

            foreach (var indoor in groupedIndoor)
            {
                var trackEvents = await db.TrackEvents
                    .Where(t => t.Name == indoor.Key.Name)
                    .OrderBy(t => t.SortOrder)
                    .ToListAsync();

                var trackEvent = trackEvents.FirstOrDefault();

                var boysBest = indoor.Key.RunningEvent ? indoor.Where(p => !p.Athlete.Gender).MinBy(p => p.TotalSeconds) : indoor.Where(p => !p.Athlete.Gender).MaxBy(p => p.TotalInches);

                var boysAthletes = new List<Athlete>();
                if (boysBest != null)
                {
                    await SetPersonalBest(boysBest);
                    await SetSeasonBest(boysBest);

                    if (trackEvent is { RelayEvent: true })
                    {
                        if (trackEvent.RunningEvent)
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == boysBest.TrackEvent.Name && p.MeetId == boysBest.MeetId && p.Minutes == boysBest.Minutes && p.Seconds == boysBest.Seconds && p.Milliseconds == boysBest.Milliseconds && !p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            boysAthletes = matching;
                        }
                        else
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == boysBest.TrackEvent.Name && p.MeetId == boysBest.MeetId && p.Feet == boysBest.Feet && p.Inches == boysBest.Inches && p.FractionalInches == boysBest.FractionalInches && !p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            boysAthletes = matching;
                        }
                    }
                    else
                    {
                        boysAthletes.Add(boysBest.Athlete);
                    }
                }

                var girlsBest = indoor.Key.RunningEvent ? indoor.Where(p => p.Athlete.Gender).MinBy(p => p.TotalSeconds) : indoor.Where(p => p.Athlete.Gender).MaxBy(p => p.TotalInches);

                var girlsAthletes = new List<Athlete>();
                if (girlsBest != null)
                {
                    await SetPersonalBest(girlsBest);
                    await SetSeasonBest(girlsBest);

                    if (trackEvent is { RelayEvent: true })
                    {
                        if (trackEvent.RunningEvent)
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == girlsBest.TrackEvent.Name && p.MeetId == girlsBest.MeetId && p.Minutes == girlsBest.Minutes && p.Seconds == girlsBest.Seconds && p.Milliseconds == girlsBest.Milliseconds && p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            girlsAthletes = matching;
                        }
                        else
                        {
                            var matching = await db.Performances
                                .Include(p => p.Athlete)
                                .Include(p => p.Meet)
                                .Include(p => p.TrackEvent)
                                .Where(p => p.TrackEvent.Name == girlsBest.TrackEvent.Name && p.MeetId == girlsBest.MeetId && p.Feet == girlsBest.Feet && p.Inches == girlsBest.Inches && p.FractionalInches == girlsBest.FractionalInches && p.Athlete.Gender)
                                .Select(p => p.Athlete)
                                .ToListAsync();

                            girlsAthletes = matching;
                        }
                    }
                    else
                    {
                        girlsAthletes.Add(girlsBest.Athlete);
                    }
                }

                var eventBest = new EventBest(trackEvent, boysBest, boysAthletes, girlsBest, girlsAthletes);
                indoorBests.Add(eventBest);
            }

            var viewModel = new SeasonDetailsViewModel(selectedSeason, outdoorMeets, indoorMeets, outdoorBests, indoorBests);
            return View(viewModel);
        }

        [Route("seasons/{seasonName}/meets")]
        public async Task<IActionResult> SeasonMeets(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            if (selectedSeason == null)
            {
                return NotFound();
            }

            var outdoorMeets = await db.Meets
                .Include(m => m.Performances)
                .Where(m => m.SeasonId == selectedSeason.Id && m.Outdoor && m.Performances.Any())
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            var indoorMeets = await db.Meets
                .Include(m => m.Performances)
                .Where(m => m.SeasonId == selectedSeason.Id && m.Outdoor && m.Performances.Any())
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            return null;
        }

        [Route("seasons/{seasonName}/leaderboard/outdoor")]
        public async Task<IActionResult> SeasonLeaderboardOutdoor(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            return selectedSeason == null ? NotFound() : null;
        }

        [Route("seasons/{seasonName}/leaderboard/outdoor/prs")]
        public async Task<IActionResult> SeasonPrLeaderboardOutdoor(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            return selectedSeason == null ? NotFound() : null;
        }

        [Route("seasons/{seasonName}/leaderboard/indoor")]
        public async Task<IActionResult> SeasonLeaderboardIndoor(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            return selectedSeason == null ? NotFound() : null;
        }

        [Route("seasons/{seasonName}/leaderboard/indoor/prs")]
        public async Task<IActionResult> SeasonPrLeaderboardIndoor(string seasonName)
        {
            var seasons = await db.Seasons.ToListAsync();
            var selectedSeason = seasons.FirstOrDefault(s => s.Name == seasonName);
            return selectedSeason == null ? NotFound() : null;
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<Dictionary<Performance, List<Athlete>>> GetPerformanceDictionary(List<Performance> performances, TrackEvent selectedEvent)
        {
            var performancesDictionary = new Dictionary<Performance, List<Athlete>>();
            performances = selectedEvent.RunningEvent
                ? performances.OrderBy(p => p.TotalSeconds).ToList()
                : performances.OrderByDescending(p => p.TotalInches).ToList();
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
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId &&
                                        p.Minutes == performance.Minutes && p.Seconds == performance.Seconds &&
                                        p.Milliseconds == performance.Milliseconds)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

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
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId &&
                                        p.Feet == performance.Feet && p.Inches == performance.Inches &&
                                        p.FractionalInches == performance.FractionalInches)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        performancesDictionary.Add(performance, matching);
                    }
                }
            }
            else
            {
                foreach (var performance in performances)
                {
                    await SetPersonalBest(performance);
                    await SetSeasonBest(performance);

                    performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                }
            }

            return performancesDictionary;
        }

        private async Task<Dictionary<Performance, List<Athlete>>> GetPrPerformances(List<Performance> performances, TrackEvent selectedEvent)
        {
            var performancesDictionary = new Dictionary<Performance, List<Athlete>>();
            performances = selectedEvent.RunningEvent
                ? performances.OrderBy(p => p.TotalSeconds).ToList()
                : performances.OrderByDescending(p => p.TotalInches).ToList();
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
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId &&
                                        p.Minutes == performance.Minutes && p.Seconds == performance.Seconds &&
                                        p.Milliseconds == performance.Milliseconds)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        var outdoorPerformances =
                            performancesDictionary.Where(x => x.Key.Meet.Outdoor && x.Value.All(matching.Contains));
                        var indoorPerformances =
                            performancesDictionary.Where(x => !x.Key.Meet.Outdoor && x.Value.All(matching.Contains));

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        if (!outdoorPerformances.Any() && performance.Meet.Outdoor)
                        {
                            performancesDictionary.Add(performance, matching);
                        }
                        else if (!indoorPerformances.Any() && !performance.Meet.Outdoor)
                        {
                            performancesDictionary.Add(performance, matching);
                        }
                        else if (!performancesDictionary.Values.Any(x => x.All(matching.Contains)))
                        {
                            performancesDictionary.Add(performance, matching);
                        }
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
                            .Where(p => p.TrackEventId == performance.TrackEventId && p.MeetId == performance.MeetId &&
                                        p.Feet == performance.Feet && p.Inches == performance.Inches &&
                                        p.FractionalInches == performance.FractionalInches)
                            .Select(p => p.Athlete)
                            .ToListAsync();

                        var outdoorPerformances =
                            performancesDictionary.Where(x => x.Key.Meet.Outdoor && x.Value.All(matching.Contains));
                        var indoorPerformances =
                            performancesDictionary.Where(x => !x.Key.Meet.Outdoor && x.Value.All(matching.Contains));

                        await SetPersonalBest(performance);
                        await SetSeasonBest(performance);

                        if (!outdoorPerformances.Any() && performance.Meet.Outdoor)
                        {
                            performancesDictionary.Add(performance, matching);
                        }
                        else if (!indoorPerformances.Any() && !performance.Meet.Outdoor)
                        {
                            performancesDictionary.Add(performance, matching);
                        }
                        else if (!performancesDictionary.Values.Any(x => x.All(matching.Contains)))
                        {
                            performancesDictionary.Add(performance, matching);
                        }
                    }
                }
            }
            else
            {
                foreach (var performance in performances)
                {
                    var outdoorPerformances = performancesDictionary.Where(x =>
                        x.Key.Meet.Outdoor && x.Value.All(new List<Athlete> { performance.Athlete }.Contains));
                    var indoorPerformances = performancesDictionary.Where(x =>
                        !x.Key.Meet.Outdoor && x.Value.All(new List<Athlete> { performance.Athlete }.Contains));

                    await SetPersonalBest(performance);
                    await SetSeasonBest(performance);

                    if (!outdoorPerformances.Any() && performance.Meet.Outdoor)
                    {
                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                    else if (!indoorPerformances.Any() && !performance.Meet.Outdoor)
                    {
                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                    else if (!performancesDictionary.Values.Any(x => x.All((new List<Athlete> { performance.Athlete }).Contains)))
                    {
                        performancesDictionary.Add(performance, new List<Athlete> { performance.Athlete });
                    }
                }
            }

            return performancesDictionary;
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
                    outdoorLifetimePr = group.Key.RunningEvent ? group.Where(p => p.Meet.Outdoor).MinBy(p => p.TotalSeconds) : group.Where(p => p.Meet.Outdoor).MaxBy(p => p.TotalInches);
                }

                if (group.Any(p => !p.Meet.Outdoor))
                {
                    indoorLifetimePr = group.Key.RunningEvent ? group.Where(p => !p.Meet.Outdoor).MinBy(p => p.TotalSeconds) : group.Where(p => !p.Meet.Outdoor).MaxBy(p => p.TotalInches);
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
                        outdoorSeasonPr = eventGroup.Key.RunningEvent ? eventGroup.Where(p => p.Meet.Outdoor).MinBy(p => p.TotalSeconds) : eventGroup.Where(p => p.Meet.Outdoor).MaxBy(p => p.TotalInches);
                    }

                    if (eventGroup.Any(p => !p.Meet.Outdoor))
                    {
                        indoorSeasonPr = eventGroup.Key.RunningEvent ? eventGroup.Where(p => !p.Meet.Outdoor).MinBy(p => p.TotalSeconds) : eventGroup.Where(p => !p.Meet.Outdoor).MaxBy(p => p.TotalInches);
                    }

                    var eventPr = new EventPr(eventGroup.Key, outdoorSeasonPr, indoorSeasonPr);
                    eventPrs.Add(eventPr);
                }

                var seasonPr = new SeasonPr(group.Key, eventPrs);
                seasonPrs.Add(seasonPr);
            }

            return seasonPrs;
        }

        private async Task<List<EventPerformance>> GetEventPerformances(Athlete athlete)
        {
            var eventPerformances = new List<EventPerformance>();

            var performancesGroupedByEvent = athlete.Performances.GroupBy(p => p.TrackEvent).OrderBy(p => p.Key.SortOrder);
            foreach (var eventGroup in performancesGroupedByEvent)
            {
                var performances = eventGroup.Key.RunningEvent ? eventGroup.OrderBy(p => p.TotalSeconds) : eventGroup.OrderByDescending(p => p.TotalInches);

                foreach (var performance in performances)
                {
                    await SetPersonalBest(performance);
                    await SetSeasonBest(performance);
                }

                var eventPerformance = new EventPerformance(eventGroup.Key, performances);
                eventPerformances.Add(eventPerformance);
            }

            return eventPerformances;
        }

        private async Task SetPersonalBest(Performance performance)
        {
            var athletePerformances = await db.Performances
                .Include(p => p.TrackEvent)
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Where(p => p.AthleteId == performance.AthleteId && p.TrackEventId == performance.TrackEventId && p.Meet.Outdoor == performance.Meet.Outdoor)
                .ToListAsync();

            if (performance.TrackEvent.RunningEvent)
            {
                var best = athletePerformances.MinBy(p => p.TotalSeconds);
                if (best != null && Math.Abs(best.TotalSeconds - performance.TotalSeconds) < .01)
                {
                    performance.IsPersonalBest = true;
                }
            }
            else
            {
                var best = athletePerformances.MaxBy(p => p.TotalInches);
                if (best != null && Math.Abs(best.TotalInches - performance.TotalInches) < .01)
                {
                    performance.IsPersonalBest = true;
                }
            }
        }

        private async Task SetSeasonBest(Performance performance)
        {
            var athletePerformances = await db.Performances
                .Include(p => p.TrackEvent)
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .Where(p => p.AthleteId == performance.AthleteId && p.TrackEventId == performance.TrackEventId && p.Meet.Outdoor == performance.Meet.Outdoor && p.Meet.SeasonId == performance.Meet.SeasonId)
                .ToListAsync();

            if (performance.TrackEvent.RunningEvent)
            {
                var best = athletePerformances.MinBy(p => p.TotalSeconds);
                if (best != null && Math.Abs(best.TotalSeconds - performance.TotalSeconds) < .01)
                {
                    performance.IsSeasonBest = true;
                }
            }
            else
            {
                var best = athletePerformances.MaxBy(p => p.TotalInches);
                if (best != null && Math.Abs(best.TotalInches - performance.TotalInches) < .01)
                {
                    performance.IsSeasonBest = true;
                }
            }
        }
    }
}
