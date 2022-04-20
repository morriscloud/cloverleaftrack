using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.ViewModels;

using Microsoft.EntityFrameworkCore;

namespace CloverleafTrack.Managers
{
    public interface IPerformanceManager
    {
        public Task<int> CountAsync();
        public List<Performance> Performances { get; }
        public List<LeaderboardPerformance> GetLeaderboardPerformances(List<TrackEvent> trackEvents);
        public List<EventLeaderboardPerformance> GetEventLeaderboardPerformances(TrackEvent trackEvent);
        public List<EventLeaderboardPerformance> GetEventLeaderboardPrPerformances(TrackEvent trackEvent);
    }

    public class PerformanceManager : IPerformanceManager
    {
        private readonly CloverleafTrackDataContext db;
        private List<Performance> cache = new();

        public PerformanceManager(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public async Task<int> CountAsync()
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.Count;
        }
        
        public List<Performance> Performances => Cache.AllPerformances;

        private async Task RefreshCacheAsync()
        {
            cache.Clear();
            cache.TrimExcess();
            cache = await db.Performances
                .AsNoTracking()
                .Include(p => p.TrackEvent)
                .Include(p => p.Athlete)
                .Include(p => p.Meet)
                .ThenInclude(m => m.Season)
                .ToListAsync();
        }

        public List<LeaderboardPerformance> GetLeaderboardPerformances(List<TrackEvent> trackEvents)
        {
            List<LeaderboardPerformance> leaderboardPerformances = new();

            foreach (var trackEvent in trackEvents)
            {
                if (Cache.LeaderboardPerformances.ContainsKey(trackEvent))
                {
                    leaderboardPerformances.Add(Cache.LeaderboardPerformances[trackEvent]);
                }
                else
                {
                    var performance = GetBestPerformanceWithAthletes(trackEvent, out var athletes);

                    var leaderboardPerformance = new LeaderboardPerformance(trackEvent, performance, athletes);
                    Cache.LeaderboardPerformances[trackEvent] = leaderboardPerformance;
                    leaderboardPerformances.Add(leaderboardPerformance);
                }
            }

            return leaderboardPerformances;
        }

        public List<EventLeaderboardPerformance> GetEventLeaderboardPerformances(TrackEvent trackEvent)
        {
            if (Cache.EventLeaderboardPerformances.ContainsKey(trackEvent))
            {
                return Cache.EventLeaderboardPerformances[trackEvent];
            }

            var eventLeaderboardPerformances = new List<EventLeaderboardPerformance>();

            var performances = trackEvent.RunningEvent
                ? trackEvent.Performances.OrderBy(p => p.TotalSeconds).ToList()
                : trackEvent.Performances.OrderByDescending(p => p.TotalInches).ToList();

            if (trackEvent.RelayEvent)
            {
                if (trackEvent.RunningEvent)
                {
                    performances = performances.DistinctBy(p => p.TotalSeconds).ToList();

                    foreach (var performance in performances)
                    {
                        var relayPerformances = trackEvent.Performances
                            .Where(p => Math.Abs(p.TotalSeconds - performance.TotalSeconds) < 0.01 && p.MeetId == performance.MeetId)
                            .ToList();

                        var eventLeaderboardPerformance = new EventLeaderboardPerformance(performance, relayPerformances.Select(p => p.Athlete).OrderBy(a => a.UrlName).ToList());
                        eventLeaderboardPerformances.Add(eventLeaderboardPerformance);
                    }
                }
                else
                {
                    performances = performances.DistinctBy(p => p.TotalInches).ToList();

                    foreach (var performance in performances)
                    {
                        var relayPerformances = trackEvent.Performances
                            .Where(p => Math.Abs(p.TotalInches - performance.TotalInches) < 0.01 && p.MeetId == performance.MeetId)
                            .ToList();

                        var eventLeaderboardPerformance = new EventLeaderboardPerformance(performance, relayPerformances.Select(p => p.Athlete).OrderBy(a => a.UrlName).ToList());
                        eventLeaderboardPerformances.Add(eventLeaderboardPerformance);
                    }
                }
            }
            else
            {
                foreach (var performance in performances)
                {
                    var eventLeaderboardPerformance = new EventLeaderboardPerformance(performance, new List<Athlete> { performance.Athlete });
                    eventLeaderboardPerformances.Add(eventLeaderboardPerformance);

                    if (Cache.AthletePrs.ContainsKey(performance.Athlete))
                    {
                        var eventPerformance = Cache.AthletePrs[performance.Athlete].FirstOrDefault(p => p.TrackEventId == performance.TrackEventId && p.Meet.Outdoor == performance.Meet.Outdoor);
                        if (eventPerformance != null)
                        {
                            if (Math.Abs(eventPerformance.TotalInches - performance.TotalInches) < 0.01 && Math.Abs(eventPerformance.TotalSeconds - performance.TotalSeconds) < 0.01)
                            {
                                performance.IsPersonalBest = true;
                            }
                        }
                    }
                    else
                    {
                        Cache.AthletePrs[performance.Athlete] = new List<Performance> { performance };
                        performance.IsPersonalBest = true;
                    }

                    if (Cache.AthleteSeasonBests.ContainsKey(performance.Athlete) && Cache.AthleteSeasonBests[performance.Athlete].ContainsKey(performance.Meet.Season))
                    {
                        var seasonBest = Cache.AthleteSeasonBests[performance.Athlete][performance.Meet.Season].FirstOrDefault(p => p.TrackEventId == performance.TrackEventId && p.Meet.Outdoor == performance.Meet.Outdoor);
                        if (seasonBest != null)
                        {
                            if (Math.Abs(seasonBest.TotalInches - performance.TotalInches) < 0.01 && Math.Abs(seasonBest.TotalSeconds - performance.TotalSeconds) < 0.01)
                            {
                                performance.IsSeasonBest = true;
                            }
                        }
                    }
                    else
                    {
                        if (!Cache.AthleteSeasonBests.ContainsKey(performance.Athlete))
                        {
                            Cache.AthleteSeasonBests[performance.Athlete] = new();
                        }

                        Cache.AthleteSeasonBests[performance.Athlete][performance.Meet.Season].Add(performance);
                    }
                }
            }

            Cache.EventLeaderboardPerformances[trackEvent] = eventLeaderboardPerformances;
            return eventLeaderboardPerformances;
        }

        public List<EventLeaderboardPerformance> GetEventLeaderboardPrPerformances(TrackEvent trackEvent)
        {
            if (Cache.EventLeaderboardPrPerformances.ContainsKey(trackEvent))
            {
                return Cache.EventLeaderboardPrPerformances[trackEvent];
            }

            var eventLeaderboardPerformances = GetEventLeaderboardPerformances(trackEvent);
            var prs = new List<EventLeaderboardPerformance>();

            foreach (var eventLeaderboardPerformance in eventLeaderboardPerformances)
            {
                if (prs.Any(p => p.Athletes.All(eventLeaderboardPerformance.Athletes.Contains)))
                {
                    // do nothing
                }
                else
                {
                    prs.Add(eventLeaderboardPerformance);
                }
            }

            Cache.EventLeaderboardPrPerformances[trackEvent] = prs;
            return prs;
        }

        private Performance GetBestPerformanceWithAthletes(TrackEvent trackEvent, out List<Athlete> athletes)
        {
            if (trackEvent.Performances.Count == 0)
            {
                athletes = new();
                return new Performance();
            }

            var performance = trackEvent.RunningEvent
                ? trackEvent.Performances.MinBy(p => p.TotalSeconds)
                : trackEvent.Performances.MaxBy(p => p.TotalInches);

            athletes = new List<Athlete>();

            if (trackEvent.RelayEvent)
            {
                var relayPerformances = trackEvent.RunningEvent
                    ? trackEvent.Performances.Where(p => performance != null && Math.Abs(p.TotalSeconds - performance.TotalSeconds) < 0.01).ToList()
                    : trackEvent.Performances.Where(p => performance != null && Math.Abs(p.TotalInches - performance.TotalInches) < 0.01).ToList();

                athletes.AddRange(relayPerformances.Select(p => p.Athlete));
            }
            else
            {
                if (performance != null) athletes.Add(performance.Athlete);
            }

            return performance;
        }
    }
}
