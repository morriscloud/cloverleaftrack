using CloverleafTrack.Data;
using CloverleafTrack.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CloverleafTrack.Managers
{
    public interface ITrackEventManager
    {
        public int Count { get; }
        public List<TrackEvent> TrackEvents { get; }
        public List<TrackEvent> BoysLeaderboardEvents { get; }
        public List<TrackEvent> GirlsLeaderboardEvents { get; }

        public Task Reload(CancellationToken cancellationToken);
    }

    public class TrackEventManager : ITrackEventManager
    {
        private readonly CloverleafTrackDataContext db;

        public TrackEventManager(CloverleafTrackDataContext db)
        {
            this.db = db;
        }

        public int Count => Cache.AllTrackEvents.Count;
        public List<TrackEvent> TrackEvents => Cache.AllTrackEvents;

        public List<TrackEvent> BoysLeaderboardEvents
        {
            get
            {
                if (!Cache.BoysLeaderboardEvents.Any())
                {
                    Cache.BoysLeaderboardEvents = TrackEvents
                        .Where(t => !t.Gender && t.Performances.Any())
                        .OrderBy(t => t.SortOrder)
                        .ToList();
                }

                return Cache.BoysLeaderboardEvents;
            }
        }

        public List<TrackEvent> GirlsLeaderboardEvents
        {
            get
            {
                if (!Cache.GirlsLeaderboardEvents.Any())
                {
                    Cache.GirlsLeaderboardEvents = TrackEvents
                        .Where(t => t.Gender && t.Performances.Any())
                        .OrderBy(t => t.SortOrder)
                        .ToList();
                }

                return Cache.GirlsLeaderboardEvents;
            }
        }

        public async Task Reload(CancellationToken cancellationToken)
        {
            Cache.AllTrackEvents = await db.TrackEvents
                .Include(t => t.Performances)
                .ThenInclude(p => p.Athlete)
                .Include(t => t.Performances)
                .ThenInclude(p => p.Meet)
                .ToListAsync(cancellationToken);
        }
    }
}