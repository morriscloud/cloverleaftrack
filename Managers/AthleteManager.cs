using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CloverleafTrack.Data;
using CloverleafTrack.Models;
using CloverleafTrack.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Managers
{
    public interface IAthleteManager
    {
        public int Count { get; }
        public List<Athlete> Athletes { get; }
        public List<Athlete> CurrentAthletes { get; }
        public List<Athlete> GraduatedAthletes { get; }
        public Task Reload(CancellationToken cancellationToken);
        public List<Performance> GetAthletePrs(Athlete athlete);
        public Dictionary<Season, List<Performance>> GetAthleteSeasonBests(Athlete athlete);
    }

    public class AthleteManager : IAthleteManager
    {
        private readonly CloverleafTrackDataContext db;
        private readonly CurrentSeasonOptions currentSeason;
        private readonly ITrackEventManager trackEventManager;
        private readonly IPerformanceManager performanceManager;

        public AthleteManager(CloverleafTrackDataContext db, ITrackEventManager trackEventManager, IPerformanceManager performanceManager, IOptions<CurrentSeasonOptions> currentSeason)
        {
            this.db = db;
            this.trackEventManager = trackEventManager;
            this.performanceManager = performanceManager;
            this.currentSeason = currentSeason.Value;
        }

        public int Count => Cache.AllAthletes.Count;
        public List<Athlete> Athletes => Cache.AllAthletes;
        public List<Athlete> CurrentAthletes => Athletes
            .Where(a => a.GraduationYear >= currentSeason.GraduationYear && a.GraduationYear <= currentSeason.GraduationYear + 3)
            .OrderBy(a => a.UrlName)
            .ToList();

        public List<Athlete> GraduatedAthletes => Athletes
            .Where(a => a.GraduationYear < currentSeason.GraduationYear)
            .OrderByDescending(a => a.GraduationYear)
            .ThenBy(a => a.UrlName)
            .ToList();

        public async Task Reload(CancellationToken cancellationToken)
        {
            Cache.AllAthletes = await db.Athletes
                .Include(a => a.Performances)
                .ThenInclude(p => p.TrackEvent)
                .Include(a => a.Performances)
                .ThenInclude(p => p.Meet)
                .ThenInclude(m => m.Season)
                .ToListAsync(cancellationToken);
        }

        public List<Performance> GetAthletePrs(Athlete athlete)
        {
            if (Cache.AthletePrs.ContainsKey(athlete))
            {
                return Cache.AthletePrs[athlete];
            }

            var prs = new List<Performance>();
            foreach (var trackEvent in trackEventManager.TrackEvents)
            {
                var performances = performanceManager.Performances
                    .Where(p => p.AthleteId == athlete.Id && p.TrackEventId == trackEvent.Id)
                    .ToList();

                if (performances.Any())
                {
                    var pr = trackEvent.RunningEvent
                        ? performances.MinBy(p => p.TotalSeconds)
                        : performances.MaxBy(p => p.TotalInches);

                    prs.Add(pr);
                }
            }

            Cache.AthletePrs[athlete] = prs;
            return prs;
        }

        public Dictionary<Season, List<Performance>> GetAthleteSeasonBests(Athlete athlete)
        {
            if (Cache.AthleteSeasonBests.ContainsKey(athlete))
            {
                return Cache.AthleteSeasonBests[athlete];
            }

            var seasonBests = new Dictionary<Season, List<Performance>>();

            foreach (var trackEvent in trackEventManager.TrackEvents)
            {
                var performances = performanceManager.Performances
                    .Where(p => p.AthleteId == athlete.Id && p.TrackEventId == trackEvent.Id)
                    .ToList();

                if (performances.Any())
                {
                    var seasonPerformances = performances.GroupBy(p => p.Meet.Season).ToList();

                    foreach (var seasonGroup in seasonPerformances)
                    {
                        var best = trackEvent.RunningEvent
                            ? seasonGroup.MinBy(p => p.TotalSeconds)
                            : seasonGroup.MaxBy(p => p.TotalInches);

                        if (seasonBests.ContainsKey(seasonGroup.Key))
                        {
                            seasonBests[seasonGroup.Key].Add(best);
                        }
                        else
                        {
                            seasonBests[seasonGroup.Key] = new List<Performance> { best };
                        }
                    }
                }
            }

            Cache.AthleteSeasonBests[athlete] = seasonBests;
            return seasonBests;
        }
    }
}