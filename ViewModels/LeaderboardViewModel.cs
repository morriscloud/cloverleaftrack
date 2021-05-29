
using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record LeaderboardViewModel(List<LeaderboardPerformance> BoysLeaderboardPerformances, List<LeaderboardPerformance> GirlsLeaderboardPerformances);
}
