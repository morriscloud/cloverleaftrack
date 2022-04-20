using System.Collections.Generic;
using System.Collections.Immutable;
using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels
{
    public record RosterViewModel(ImmutableList<Athlete> CurrentAthletes, ImmutableList<Athlete> GraduatedAthletes);
}
