using System;
using System.Collections.Generic;

namespace CloverleafTrack.Models
{
    public class Athlete
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public int GraduationYear { get; set; }

        public List<Performance> Performances { get; set; } = new List<Performance>();
    }
}
