using System;
using System.Collections.Generic;

namespace CloverleafTrack.Models
{
    public class Meet
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int CloverleafPlace { get; set; }
        public Guid SeasonId { get; set; }

        public Season Season { get; set; }
        public List<School> Schools { get; set; } = new List<School>();
        public List<Performance> Performances { get; set; } = new List<Performance>();
    }
}
