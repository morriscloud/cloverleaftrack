using CloverleafTrack.Models;
using CloverleafTrack.Models.TrackEvents;

using Microsoft.EntityFrameworkCore;

namespace CloverleafTrack.Data
{
    public class CloverleafTrackDataContext : DbContext
    {
        public CloverleafTrackDataContext(DbContextOptions<CloverleafTrackDataContext> options)
            : base(options)
        {
        }

        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Meet> Meets { get; set; }
        public DbSet<TrackEvent> TrackEvents { get; set; }
        public DbSet<FieldEvent> FieldEvents { get; set; }
        public DbSet<FieldRelayEvent> FieldRelayEvents { get; set; }
        public DbSet<RunningEvent> RunningEvents { get; set; }
        public DbSet<RunningRelayEvent> RunningRelayEvents { get; set; }
        public DbSet<Performance> Performances { get; set; }
    }
}
