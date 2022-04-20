using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CloverleafTrack.Managers
{
    public interface ISeasonManager
    {
        public Task<int> CountAsync();
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
        
        public List<Season> Seasons => Cache.AllSeasons;

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