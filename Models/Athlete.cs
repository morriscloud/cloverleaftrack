using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace CloverleafTrack.Models
{
    public class Athlete
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public int GraduationYear { get; set; }
        [NotMapped] public string Name => $"{FirstName} {LastName}";

        [NotMapped] public string UrlName => $"{HttpUtility.UrlEncode(FirstName.ToLower())}-{HttpUtility.UrlEncode(LastName.ToLower())}";

        public List<Performance> Performances { get; set; } = new();
    }
}
