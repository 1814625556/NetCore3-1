using System;
using System.Collections.Generic;
using System.Text;

namespace Rx.Models
{
    public class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
}
