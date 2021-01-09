using System.Collections.Generic;

using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record RosterViewModel(List<Athlete> CurrentAthletes, List<Athlete> GraduatedAthletes);
}
