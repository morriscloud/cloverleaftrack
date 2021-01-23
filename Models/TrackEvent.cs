using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace CloverleafTrack.Models
{
    public class TrackEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public bool RunningEvent { get; set; }
        public bool RelayEvent { get; set; }
        public int SortOrder { get; set; }
        [NotMapped] public string DisplayName => Gender ? $"Girls {Name}" : $"Boys {Name}";
        [NotMapped] public string UrlName => Gender ? $"girls-{HttpUtility.UrlEncode(Name.Replace(" ", "-").ToLower())}" : $"boys-{HttpUtility.UrlEncode(Name.Replace(" ", "-").ToLower())}";

        public List<Performance> Performances { get; set; } = new();
    }
}
