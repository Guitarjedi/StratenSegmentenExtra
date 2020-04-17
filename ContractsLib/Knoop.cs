using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class Knoop
    {
        public Knoop(int knoopId, Punt punt)
        {
            Id = knoopId;
            Punt = punt;
        }
        public Knoop() { Punt = new Punt(); }

        public int Id { get; set; }
        public Punt Punt { get; set; }

        public override string ToString() => $"KnoopID: {Id}, Punt: {Punt}";
        

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Knoop k = (Knoop)obj;
                return (Id == k.Id);
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Punt);
        }
    }
}
