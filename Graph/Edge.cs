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

        public Edge()
        {

        }

        public Edge(int from, int to)
        {
            Node1 = from;
            Node2 = to;
        }

        public Edge(int from, int to, int cost)
        {
            Node1 = from;
            Node2 = to;
            Cost = cost;
        }

        public override string ToString()
        {
            return string.Format("{0} <--> {1} = {2}", Node1, Node2, Cost);
        }
    }
}
