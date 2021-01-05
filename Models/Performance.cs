using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloverleafTrack.Models
{
    public class Performance
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public bool RunningEvent { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }
        public int Feet { get; set; }
        public int Inches { get; set; }
        public int FrationalInches { get; set; }
        public int Place { get; set; }
        public Guid AthleteId { get; set; }
        public Guid MeetId { get; set; }

        public Athlete Athlete { get; set; }
        public Meet Meet { get; set; }
    }
}
