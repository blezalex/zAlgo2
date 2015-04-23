using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLib
{
    public class Graph
    {
        public List<Node> Nodes;
        public List<Edge> Edges;

        public static Graph ReadFromFile(string filename)
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();

            using (var sr = File.OpenText(filename))
            {
                var infoLine = sr.ReadLine();
                var infoParts = infoLine.Split(' ');
                int nodeCnt = int.Parse(infoParts[0]);

                for (var i = 0; i < nodeCnt; i++)
                {
                    nodes.Add(new Node());
                }


                string line;
                int edgeIdx = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(' ');

                    var edge = new Edge { Node1 = int.Parse(parts[0]) - 1, Node2 = int.Parse(parts[1]) - 1, Cost = int.Parse(parts[2]) };
                    edges.Add(edge);
                    edgeIdx++;

                    nodes[edge.Node1].Edges.Add(edge);
                    if (edge.Node1 != edge.Node2)
                    {
                        nodes[edge.Node2].Edges.Add(edge);
                    }
                }
            }

            return new Graph { Nodes = nodes, Edges = edges };
        }
    }
}
