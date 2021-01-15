using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record LeaderboardViewModel(Dictionary<TrackEvent, Performance> BoysEvents, Dictionary<TrackEvent, Performance> GirlsEvents);
}
