using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record EventBest(TrackEvent TrackEvent, Performance BoysBest, List<Athlete> BoysAthletes, Performance GirlsBest, List<Athlete> GirlsAthletes);
}