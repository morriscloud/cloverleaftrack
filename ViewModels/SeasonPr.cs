
using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record SeasonPr(Season Season, List<EventPr> EventPrs);
}
