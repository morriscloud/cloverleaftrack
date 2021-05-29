using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record EventLeaderboardViewModel(TrackEvent TrackEvent, List<EventLeaderboardPerformance> Performances, bool Prs);
}