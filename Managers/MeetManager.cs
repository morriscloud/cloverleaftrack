using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CloverleafTrack.Data;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using CloverleafTrack.Models;

namespace CloverleafTrack.Managers
{
    public interface IMeetManager
    {
        public Task<int> CountAsync();
        public Task<ImmutableList<Meet>> GetAsync();
        public Task<Meet> GetByIdAsync(Guid id);
        public Task DoneAsync(Guid id);
    }

    public class MeetManager : IMeetManager
    {
        private readonly CloverleafTrackDataContext db;
        private List<Meet> cache = new();

        public MeetManager(CloverleafTrackDataContext db)
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

        public async Task<ImmutableList<Meet>> GetAsync()
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.OrderByDescending(m => m.Date).ToImmutableList();
        }

        public async Task<Meet> GetByIdAsync(Guid id)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            return cache.FirstOrDefault(m => m.Id == id);
        }
        
        public async Task DoneAsync(Guid id)
        {
            if (!cache.Any())
            {
                await RefreshCacheAsync();
            }

            var meet = await db.Meets.FindAsync(id);
            if (meet != null)
            {
                meet.AllResultsIn = true;

                db.Update(meet);
                await db.SaveChangesAsync();
                db.Entry(meet).State = EntityState.Detached;
                await RefreshCacheAsync();
            }
        }

        private async Task RefreshCacheAsync()
        {
            cache.Clear();
            cache.TrimExcess();
            cache = await db.Meets
                .AsNoTracking()
                .Include(m => m.Season)
                .Include(m => m.Performances)
                .ThenInclude(p => p.TrackEvent)
                .Include(m => m.Performances)
                .ThenInclude(p => p.Athlete)
                .ToListAsync();
        }
    }
}