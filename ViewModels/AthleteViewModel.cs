using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record AthleteViewModel(Athlete Athlete, List<LifetimePr> LifetimePrs, List<SeasonPr> SeasonPrs, List<EventPerformance> Performances);
}
