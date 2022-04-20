using System;
using System.Collections.Generic;

namespace CloverleafTrack.Models
{
    public class Season : IEquatable<Season>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Meet> Meets { get; set; } = new();

        public bool Equals(Season other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Season) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
