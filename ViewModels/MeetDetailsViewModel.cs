using System.Collections.Generic;
using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record MeetDetailsViewModel(Meet Meet, Dictionary<TrackEvent, List<Performance>> BoysResults, Dictionary<TrackEvent, List<Performance>> GirlsResults);
}