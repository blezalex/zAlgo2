using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;
using System.Diagnostics;

namespace hw4
{
    class Program
    {
        static void Main1(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\large.txt");

            var dummyNodeId = AddDummyNode(graph);

            var sw = new Stopwatch();

            var shortestPath = BellmandFordShortestPath.Compute(dummyNodeId, graph);
            // REweight

            // Deijstra n times

            // fix path len

            sw.Stop();
            Console.WriteLine("{0} {1}", 0, sw.Elapsed);
        }

        private static int AddDummyNode(Graph graph)
        {
            var dummyNode = new Node();
            var dummyNodeId = graph.Nodes.Count;

            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                var node = graph.Nodes[i];
                var edge = new Edge() { Cost = 0, Node1 = dummyNodeId, Node2 = i };
                node.Edges.Add(edge);
                dummyNode.Edges.Add(edge);
            }

            graph.Nodes.Add(dummyNode);
            return dummyNodeId;
        }

        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\g3.txt");
            
            var sw = new Stopwatch();  
            // 29.2 sec for naive bf * n
            // 16 sec bf * n    memory optimized
            // 7.9 FW   memory optimized
            // 7.36 FW count optimized
            sw.Start();

            var shortestPathInGraph = CalcPathWithNBF(graph);

            sw.Stop();

            Console.WriteLine("{0} {1}", shortestPathInGraph, sw.Elapsed);
        }

        private static int CalcPathWithFloydWarshal(Graph graph)
        {
            var shortestPath = FloydWarshallShortestPath.Compute(graph);

            var shortestPathInGraph = int.MaxValue;
            for (int sourceNodeId = 0; sourceNodeId < graph.Nodes.Count; sourceNodeId++)
            {
                for (int destNodeId = 0; destNodeId < graph.Nodes.Count; destNodeId++)
                {
                    shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPath[sourceNodeId, destNodeId]);
                }   
            }

            return shortestPathInGraph;
        }

        private static int CalcPathWithNBF(Graph graph)
        {
            var shortestPathInGraph = int.MaxValue;
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var shortestPath = BellmandFordShortestPath.Compute(nodeId, graph);
                var shortestPathFromCurrentStartNode = shortestPath.Min();

                shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPathFromCurrentStartNode);
            }

            return shortestPathInGraph;
        }
    }
}
