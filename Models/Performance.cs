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
                        int minutes;
                        if (Minutes is null)
                        {
                            minutes = 0;
                        }
                        else
                        {
                            minutes = Minutes.Value;
                        }

                        return $"{minutes:00}:{Seconds:00}.{Milliseconds:00}";
                    }

                    if (TrackEvent.Name.Contains("Discus"))
                    {
                        return $"{Feet:000}-{Inches:00}.{FractionalInches:00}";
                    }
                    else
                    {
                        return $"{Feet:00}-{Inches:00}.{FractionalInches:00}";
                    }
                }

                return string.Empty;
            }
        }

        public TrackEvent TrackEvent { get; set; }
        public Athlete Athlete { get; set; }
        public Meet Meet { get; set; }
    }
}
