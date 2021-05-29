using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record EventLeaderboardPerformance(Performance Performance, List<Athlete> Athletes);
}