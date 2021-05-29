using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record LeaderboardPerformance(TrackEvent TrackEvent, Performance Performance, List<Athlete> Athletes);
}