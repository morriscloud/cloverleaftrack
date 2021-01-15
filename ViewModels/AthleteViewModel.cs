using CloverleafTrack.Models;

using System.Collections.Generic;
using System.Linq;

namespace CloverleafTrack.ViewModels
{
    public record AthleteViewModel(Athlete Athlete, Dictionary<TrackEvent, Performance> LifetimePrs, Dictionary<Season, Dictionary<TrackEvent, Performance>> SeasonPrs, Dictionary<TrackEvent, IOrderedEnumerable<Performance>> EventPerformances);
}
