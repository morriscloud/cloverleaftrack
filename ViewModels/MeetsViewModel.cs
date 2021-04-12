using System;
using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record MeetsViewModel(List<Tuple<Season, List<Meet>, List<Meet>>> Seasons);
}
