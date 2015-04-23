using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLib
{
    public class Edge
    {
        public int Node1;
        public int Node2;
        public int Cost;

        public override string ToString()
        {
            return string.Format("{0} <--> {1} = {2}", Node1, Node2, Cost);
        }
    }
}
