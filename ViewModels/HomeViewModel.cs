using System;

namespace CloverleafTrack.ViewModels
{
    public record HomeViewModel(int PerformanceCount, int MeetCount, int SeasonCount, int AthleteCount, DateTime LastUpdated);
}