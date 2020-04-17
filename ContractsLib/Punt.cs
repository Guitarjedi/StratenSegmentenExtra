using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class Punt
    {
        public Punt(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Punt() { }

        public double X { get; set; }
        public double Y { get; set; }
        public override bool Equals(Object obj)
        {
            if((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Punt p = (Punt)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"X: {X.ToString()}, Y: {Y.ToString()}";
        }
        
    }
}
