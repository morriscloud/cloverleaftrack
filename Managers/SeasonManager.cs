using System;
using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CloverleafTrack.Managers
{
    public interface ISeasonManager
    {
        public Task<int> CountAsync();
        public Task<ImmutableList<Season>> GetAsync();
        public Task<Season> GetByIdAsync(Guid id);
        public Task CreateAsync(Season season);
        public Task EditAsync(Guid id, string name);
        public Task DeleteAsync(Guid id);
        public bool CheckExistenceById(Guid id);
    }

    public class SeasonManager : ISeasonManager
    {
        private readonly CloverleafTrackDataContext db;
        private List<Season> cache = new();

        public SeasonManager(CloverleafTrackDataContext db)
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

        public async Task<ImmutableList<Season>> GetAsync()
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache
                .OrderByDescending(s => s.Name)
                .ToImmutableList(); 
        }

        public async Task<Season> GetByIdAsync(Guid id)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.FirstOrDefault(s => s.Id == id);
        }
        
        public async Task CreateAsync(Season season)
        {
            season.Id = Guid.NewGuid();
            db.Seasons.Add(season);
            await db.SaveChangesAsync();
            db.Entry(season).State = EntityState.Detached;
            await RefreshCacheAsync();
        }
        
        public async Task EditAsync(Guid id, string name)
        {
            var season = await db.Seasons.FindAsync(id);
            if (season != null)
            {
                season.Name = name;
                
                db.Update(season);
                await db.SaveChangesAsync();
                db.Entry(season).State = EntityState.Detached;
                await RefreshCacheAsync();                
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var season = await GetByIdAsync(id);
            db.Seasons.Remove(season);
            await db.SaveChangesAsync();
            db.Entry(season).State = EntityState.Detached;
            await RefreshCacheAsync();
        }

        public bool CheckExistenceById(Guid id)
        {
            return cache.Any(a => a.Id == id);
        }

        private async Task RefreshCacheAsync()
        {
            cache.Clear();
            cache.TrimExcess();
            cache = await db.Seasons
                .AsNoTracking()
                .Include(s => s.Meets)
                .ThenInclude(m => m.Performances)
                .ThenInclude(p => p.Athlete)
                .Include(s => s.Meets)
                .ThenInclude(m => m.Performances)
                .ThenInclude(p => p.TrackEvent)
                .ToListAsync();
        }
    }
}