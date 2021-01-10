using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record AthleteViewModel(Athlete Athlete, Dictionary<TrackEvent, Performance> LifetimePrs, Dictionary<Meet, List<Performance>> MeetResults);
}
