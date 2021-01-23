using System;
using System.Collections.Generic;

namespace CloverleafTrack.Models
{
    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Meet> Meets { get; set; } = new();
    }
}
