using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLib
{
    public class Node
    {
        public List<Edge> Edges = new List<Edge>();

        public override string ToString()
        {
            return base.ToString() + GetHashCode();
        }
    }
}
