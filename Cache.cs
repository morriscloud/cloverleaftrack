using CloverleafTrack.Models;
using CloverleafTrack.ViewModels;

using System.Collections.Generic;

namespace CloverleafTrack
{
    public static class Cache
    {
        public static List<Performance> AllPerformances { get; set; } = new();
        public static List<Season> AllSeasons { get; set; } = new();
        public static List<TrackEvent> AllTrackEvents { get; set; } = new();

        public static List<TrackEvent> BoysLeaderboardEvents { get; set; } = new();
        public static List<TrackEvent> GirlsLeaderboardEvents { get; set; } = new();

        public static Dictionary<TrackEvent, LeaderboardPerformance> LeaderboardPerformances { get; set; } = new();
        public static Dictionary<TrackEvent, List<EventLeaderboardPerformance>> EventLeaderboardPerformances { get; set; } = new();
        public static Dictionary<TrackEvent, List<EventLeaderboardPerformance>> EventLeaderboardPrPerformances { get; set; } = new();
        public static Dictionary<Athlete, List<Performance>> AthletePrs { get; set; } = new();
        public static Dictionary<Athlete, Dictionary<Season, List<Performance>>> AthleteSeasonBests { get; set; } = new();
    }
}