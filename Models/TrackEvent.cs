using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (Gender)
                {
                    return $"Female {Name}";
                }

                return $"Male {Name}";
            }
        }

        public List<Performance> Performances { get; set; }
    }
}
