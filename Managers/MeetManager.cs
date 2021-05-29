using CloverleafTrack.Data;

using Microsoft.EntityFrameworkCore;

using System.Threading;
using System.Threading.Tasks;

namespace CloverleafTrack.Managers
{
    public interface IMeetManager
    {
        public int Count { get; }
        public Task Reload(CancellationToken cancellationToken);
    }

    public class MeetManager : IMeetManager
    {
        private readonly CloverleafTrackDataContext db;

        public MeetManager(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public int Count => Cache.AllMeets.Count;

        public async Task Reload(CancellationToken cancellationToken)
        {
            Cache.AllMeets = await db.Meets
                .Include(m => m.Season)
                .Include(m => m.Performances)
                .ThenInclude(p => p.TrackEvent)
                .Include(m => m.Performances)
                .ThenInclude(p => p.Athlete)
                .ToListAsync(cancellationToken);
        }
    }
}