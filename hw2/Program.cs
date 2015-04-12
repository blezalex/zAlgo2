using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2
{
    class Edge
    {
        public int Node1;
        public int Node2;
        public int Cost;

        public override string ToString()
        {
            return string.Format("{0} <--> {1} = {2}", Node1, Node2, Cost);
        }
    }

    class Node
    {
        public List<Edge> Edges = new List<Edge>();

        public Node Leader;

        public int SetSize = 1; // value is valid only for leader

        public Node()
        {
            Leader = this;
        }

        public override string ToString()
        {
 	         return base.ToString() + GetHashCode();
        }
    }

    class Graph
    {
        public List<Node> Nodes;
        public List<Edge> Edges;
    }

    class Program
    {
        static Graph ReadGraph(string filename)
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

        static void Main(string[] args)
        {
            var graphFromFile = ReadGraph(@"C:\Users\Alex\Desktop\clustering1.txt");

            List<Node> graph = graphFromFile.Nodes;
            List<Edge> graphEdges = graphFromFile.Edges;

            //var graph = new List<Node> { new Node(), new Node(), new Node(), new Node(), new Node(), new Node() };
            //var graphEdges = new List<Edge> { 
            //    new Edge { Node1 = 0, Node2 = 1, Cost = 1 },
            //    new Edge { Node1 = 1, Node2 = 2, Cost = 1 },
            //    new Edge { Node1 = 2, Node2 = 3, Cost = 3 },
            //    new Edge { Node1 = 3, Node2 = 4, Cost = 2 },
            //    new Edge { Node1 = 4, Node2 = 5, Cost = 2 },
            //    new Edge { Node1 = 0, Node2 = 5, Cost = 4 },
            //};

            int numberOfClusters = graph.Count;
            int k = 4; // desired max number of clusters

            var sortedEdgesForKraskal = graphEdges.OrderBy(e => e.Cost).GetEnumerator();

            while (numberOfClusters > k)
            {
                var pairOfDisjoinedPoints = SearchForNextClosestDisjointPair(sortedEdgesForKraskal, graph);

                Join(pairOfDisjoinedPoints, graph);

                Trace.WriteLine(pairOfDisjoinedPoints.Item3);
                numberOfClusters--;

//                var countOfDistinctLeaders = graph.GroupBy(n => n.Leader).Count();

                if (numberOfClusters <= k)
                    break;
            }

            var minDistanceBetweenDisjoinedItems = SearchForNextClosestDisjointPair(sortedEdgesForKraskal, graph).Item3.Cost;

        }

        private static void Join(Tuple<Node, Node, Edge> pairOfDisjoinedPoints, List<Node> graph)
        {
            Node biggerSetNode;
            Node smallerSetNode;
            if (pairOfDisjoinedPoints.Item1.Leader.SetSize > pairOfDisjoinedPoints.Item2.Leader.SetSize)
            {
                biggerSetNode = pairOfDisjoinedPoints.Item1;
                smallerSetNode = pairOfDisjoinedPoints.Item2;
            }
            else
            {
                biggerSetNode = pairOfDisjoinedPoints.Item2;
                smallerSetNode = pairOfDisjoinedPoints.Item1;
            }

            // TODO: improve merging code
            Node leaderToReplace = smallerSetNode.Leader;
            
            foreach (var node in graph)
            {
                if (node.Leader == leaderToReplace)
                {
                    node.Leader = biggerSetNode.Leader;
                    biggerSetNode.Leader.SetSize++;
                }
            }

          //  var actualClusterSize = graph.Count(n => n.Leader == biggerSetNode.Leader);
        }

        private static Tuple<Node, Node, Edge> SearchForNextClosestDisjointPair(IEnumerator<Edge> sortedEdgesForKraskal, List<Node> graph)
        {
            while (sortedEdgesForKraskal.MoveNext())
            {
                var currentEdge = sortedEdgesForKraskal.Current;

                var node1 = graph[currentEdge.Node1];
                var node2 = graph[currentEdge.Node2];

                if (node1.Leader != node2.Leader) // is in the different set?
                {
                    return Tuple.Create(node1, node2, currentEdge);
                }

            }

            return null;
        }
    }
}
