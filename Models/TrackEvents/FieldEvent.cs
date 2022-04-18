using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace CloverleafTrack.Models.TrackEvents
{
    public class FieldEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public int SortOrder { get; set; }
        public Environment Environment { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        
        [NotMapped]
        public string DisplayName => Gender == Gender.Female ? $"Girls {Name}" : $"Boys {Name}";
        [NotMapped]
        public string UrlName => Gender == Gender.Female ? $"girls-{HttpUtility.UrlEncode(Name.Replace(" ", "-").ToLower())}" : $"boys-{HttpUtility.UrlEncode(Name.Replace(" ", "-").ToLower())}";
        
        public List<Performance> Performances { get; set; }
    }
}
