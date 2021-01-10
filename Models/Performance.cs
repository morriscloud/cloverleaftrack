using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloverleafTrack.Models
{
    public class Performance
    {
        public Guid Id { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
        public int? Milliseconds { get; set; }
        public int? Feet { get; set; }
        public int? Inches { get; set; }
        public int? FractionalInches { get; set; }
        public Guid TrackEventId { get; set; }
        public Guid AthleteId { get; set; }
        public Guid MeetId { get; set; }
        [Display(Name = "Performance")]
        [NotMapped]
        public string PerformanceFormatted
        {
            get
            {
                if (TrackEvent is not null)
                {
                    if (TrackEvent.RunningEvent)
                    {
                        var minutes = (float)Minutes.GetValueOrDefault(0);
                        var seconds = (float)Seconds.GetValueOrDefault(0);
                        var milliseconds = (float)Milliseconds.GetValueOrDefault(0);

                        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
                    }

                    var feet = (float)Feet.GetValueOrDefault(0);
                    var inches = (float)Inches.GetValueOrDefault(0);
                    var fractionalInches = (float)FractionalInches.GetValueOrDefault(0);

                    if (TrackEvent.Name.Contains("Discus"))
                    {
                        return $"{feet:000}-{inches:00}.{fractionalInches:00}";
                    }
                    else
                    {
                        return $"{feet:00}-{inches:00}.{fractionalInches:00}";
                    }
                }

                return string.Empty;
            }
        }
        [NotMapped]
        public float TotalSeconds
        {
            get
            {
                if (TrackEvent is not null)
                {
                    if (TrackEvent.RunningEvent)
                    {
                        var minutes = (float)Minutes.GetValueOrDefault(0);
                        var seconds = (float)Seconds.GetValueOrDefault(0);
                        var milliseconds = (float)Milliseconds.GetValueOrDefault(0);

                        return (minutes * 60f) + seconds + (milliseconds / 100f);
                    }
                }

                return float.MaxValue;
            }
        }
        [NotMapped]
        public float TotalInches
        {
            get
            {
                if (TrackEvent is not null)
                {
                    if (!TrackEvent.RunningEvent)
                    {
                        var feet = (float)Feet.GetValueOrDefault(0);
                        var inches = (float)Inches.GetValueOrDefault(0);
                        var fractionalInches = (float)FractionalInches.GetValueOrDefault(0);

                        return (feet * 12f) + inches + (fractionalInches / 100f);
                    }
                }

                return float.MaxValue;
            }
        }
        public TrackEvent TrackEvent { get; set; }
        public Athlete Athlete { get; set; }
        public Meet Meet { get; set; }
    }
}
