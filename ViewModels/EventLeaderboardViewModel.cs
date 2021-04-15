using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record EventLeaderboardViewModel(TrackEvent TrackEvent, Dictionary<Performance, List<Athlete>> Performances, bool Prs);
}