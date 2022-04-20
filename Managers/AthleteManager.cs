using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public Task<int> CountAsync();
        public Task<ImmutableList<Athlete>> GetOrderedAthletesAsync();
        public Task<ImmutableList<Athlete>> GetAthletesAsync(GraduationStatus graduationStatus);
        public Task<Athlete> GetAthleteByIdAsync(Guid id);
        public Task<Athlete> GetAthleteByNameAsync(string firstName, string lastName);
        public List<Performance> GetAthletePrs(Athlete athlete);
        public Dictionary<Season, List<Performance>> GetAthleteSeasonBests(Athlete athlete);
        public Task CreateAthleteAsync(Athlete athlete);
        public Task EditAthleteAsync(Guid id, string firstName, string lastName, bool gender, int graduationYear);
        public Task DeleteAthleteAsync(Guid id);
        public bool AthleteExists(Guid id);
    }

    public class AthleteManager : IAthleteManager
    {
        private readonly CloverleafTrackDataContext db;
        private List<Athlete> cache = new();
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

        public async Task<int> CountAsync()
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.Count;
        }

        public async Task<ImmutableList<Athlete>> GetOrderedAthletesAsync()
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ThenBy(a => a.GraduationYear)
                .ToImmutableList();
        }

        public async Task<ImmutableList<Athlete>> GetAthletesAsync(GraduationStatus graduationStatus)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            if (graduationStatus.HasFlag(GraduationStatus.Graduated) && graduationStatus.HasFlag(GraduationStatus.InSchool))
            {
                return cache.ToImmutableList();
            }

            if (graduationStatus.HasFlag(GraduationStatus.Graduated) && !graduationStatus.HasFlag(GraduationStatus.InSchool))
            {
                return cache
                    .Where(a => a.GraduationYear < currentSeason.GraduationYear)
                    .OrderByDescending(a => a.GraduationYear)
                    .ThenBy(a => a.UrlName)
                    .ToImmutableList();
            }

            return cache
                .Where(a => a.GraduationYear >= currentSeason.GraduationYear && a.GraduationYear <= currentSeason.GraduationYear + 3)
                .OrderBy(a => a.UrlName)
                .ToImmutableList();
        }

        public async Task<Athlete> GetAthleteByIdAsync(Guid id)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.FirstOrDefault(a => a.Id == id);
        }

        public async Task<Athlete> GetAthleteByNameAsync(string firstName, string lastName)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }
            
            return cache.FirstOrDefault(a => string.Equals(firstName, a.FirstName, StringComparison.OrdinalIgnoreCase) && string.Equals(lastName, a.LastName, StringComparison.OrdinalIgnoreCase));
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

        public async Task CreateAthleteAsync(Athlete athlete)
        {
            athlete.Id = Guid.NewGuid();
            db.Athletes.Add(athlete);
            await db.SaveChangesAsync();
            db.Entry(athlete).State = EntityState.Detached;
            await RefreshCacheAsync();
        }

        public async Task EditAthleteAsync(Guid id, string firstName, string lastName, bool gender, int graduationYear)
        {
            var athlete = await db.Athletes.FindAsync(id);
            if (athlete != null)
            {
                athlete.FirstName = firstName;
                athlete.LastName = lastName;
                athlete.Gender = gender;
                athlete.GraduationYear = graduationYear;
                
                db.Update(athlete);
                await db.SaveChangesAsync();
                db.Entry(athlete).State = EntityState.Detached;
                await RefreshCacheAsync();                
            }
        }

        public async Task DeleteAthleteAsync(Guid id)
        {
            var athlete = await GetAthleteByIdAsync(id);
            db.Athletes.Remove(athlete);
            await db.SaveChangesAsync();
            db.Entry(athlete).State = EntityState.Detached;
            await RefreshCacheAsync();
        }

        public bool AthleteExists(Guid id)
        {
            return cache.Any(a => a.Id == id);
        }
        
        private async Task RefreshCacheAsync()
        {
            cache.Clear();
            cache.TrimExcess();
            cache = await db
                .Athletes
                .AsNoTracking()
                .Include(a => a.Performances)
                .ThenInclude(p => p.TrackEvent)
                .Include(a => a.Performances)
                .ThenInclude(p => p.Meet)
                .ThenInclude(m => m.Season)
                .ToListAsync();
        }
    }
}