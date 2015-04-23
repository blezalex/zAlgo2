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
        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\g3.txt");
            
            var sw = new Stopwatch();  
            // 29.2 sec for naive bf * n
            // 16 sec bf * n    memory optimized
            // 7.9 FW   memory optimized
            sw.Start();

            var shortestPathInGraph = CalcPathWithFloydWarshal(graph);

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
