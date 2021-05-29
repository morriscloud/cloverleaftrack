using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CloverleafTrack.Managers
{
    public interface ISeasonManager
    {
        public int Count { get; }
        public List<Season> Seasons { get; }
        public Task Reload(CancellationToken cancellationToken);
    }

    public class SeasonManager : ISeasonManager
    {
        private readonly CloverleafTrackDataContext db;

        public SeasonManager(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public int Count => Cache.AllSeasons.Count;
        public List<Season> Seasons => Cache.AllSeasons;

        public async Task Reload(CancellationToken cancellationToken)
        {
            Cache.AllSeasons = await db.Seasons
                .Include(s => s.Meets)
                .ThenInclude(m => m.Performances)
                .ThenInclude(p => p.Athlete)
                .Include(s => s.Meets)
                .ThenInclude(m => m.Performances)
                .ThenInclude(p => p.TrackEvent)
                .ToListAsync(cancellationToken);
        }
    }
}