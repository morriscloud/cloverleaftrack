using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record LeaderboardViewModel(Dictionary<TrackEvent, KeyValuePair<Performance, List<Athlete>>> BoysEvents, Dictionary<TrackEvent, KeyValuePair<Performance, List<Athlete>>> GirlsEvents);
}
