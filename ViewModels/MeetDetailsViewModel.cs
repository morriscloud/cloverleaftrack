using CloverleafTrack.Models;

using System.Collections.Generic;

namespace CloverleafTrack.ViewModels
{
    public record MeetDetailsViewModel(Meet Meet, Dictionary<TrackEvent, Dictionary<Performance, List<Athlete>>> BoysResults, Dictionary<TrackEvent, Dictionary<Performance, List<Athlete>>> GirlsResults);
}