using System.Collections.Generic;
using System.Linq;
using CloverleafTrack.Data;

using Microsoft.EntityFrameworkCore;

using System.Threading;
using System.Threading.Tasks;
using CloverleafTrack.Models;

namespace CloverleafTrack.Managers
{
    public interface IMeetManager
    {
        public Task<int> CountAsync();
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