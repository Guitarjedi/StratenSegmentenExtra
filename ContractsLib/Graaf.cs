using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class Graaf
    {
        public int Id { get; set; }
        public List<Segment> Map { get; set; }
        public Graaf() { Map = new List<Segment>(); }
    }
}
