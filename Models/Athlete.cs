using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloverleafTrack.Models
{
    public class Athlete
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public int GraduationYear { get; set; }
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        public List<Performance> Performances { get; set; } = new List<Performance>();
    }
}
