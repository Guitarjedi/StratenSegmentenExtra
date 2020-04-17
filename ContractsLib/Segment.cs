using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class Segment
    {
        public Segment(int segmentId, Knoop beginKnoop, Knoop eindKnoop, List<Punt> vertices)
        {
            BeginKnoop = beginKnoop;
            EindKnoop = eindKnoop;
            Vertices = vertices;
            Id = segmentId;
        }
        public Segment() { Vertices = new List<Punt>(); BeginKnoop = new Knoop(); EindKnoop = new Knoop(); }
        public int Id { get; set; }
        public Knoop BeginKnoop { get; set; }
        public Knoop EindKnoop { get; set; }
        public List<Punt> Vertices { get; set; }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Segment s = (Segment)obj;
                return (Id.Equals(s.Id));
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BeginKnoop, EindKnoop, Vertices);
        }
    }
}
