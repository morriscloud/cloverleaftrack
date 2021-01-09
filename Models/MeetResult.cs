using System;

namespace CloverleafTrack.Models
{
    public class MeetResult
    {
        public Guid Id { get; set; }
        public int CloverleafPlace { get; set; }
        public Guid MeetId { get; set; }

        public Meet Meet { get; set; }
    }
}
