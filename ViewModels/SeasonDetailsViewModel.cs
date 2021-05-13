using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record SeasonDetailsViewModel(Season Season, List<Meet> OutdoorMeets, List<Meet> IndoorMeets, List<EventBest> OutdoorBests, List<EventBest> IndoorBests);
}