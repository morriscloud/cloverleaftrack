using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record EventLeaderboardViewModel(TrackEvent TrackEvent, Dictionary<Performance, List<Athlete>> Performances);
}