using System.Linq;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record EventPerformance(TrackEvent TrackEvent, IOrderedEnumerable<Performance> Performances);
}
