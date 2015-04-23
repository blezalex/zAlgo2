using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;

namespace hw4
{
    class Program
    {
        private static int[] BellmanFord(int sourceVertexIdx, Graph graph)
        {
            var shortestPath = new int[graph.Nodes.Count, graph.Nodes.Count + 1];
            InitWithInfiniteLength(shortestPath);

            shortestPath[sourceVertexIdx, 0] = 0; // path from source vertex to source vertes with length 0 is empty path with 0 cost

            bool pathChanged = true;
            int maxPathLen;

            for (maxPathLen = 1; maxPathLen <= graph.Nodes.Count; maxPathLen++)
            {
                pathChanged = false;

                for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
                {
                    var shortestPathUsingNewEdges = int.MaxValue;
                    foreach (var edge in graph.Nodes[nodeId].Edges)
                    {
                        if (edge.Node2 != nodeId) // ignore outgoing edges
                            continue;


                        var shortestPathToBeginingOfEdge = shortestPath[edge.Node1, maxPathLen - 1];
                        if (shortestPathToBeginingOfEdge == int.MaxValue)
                            continue;

                        shortestPathUsingNewEdges = Math.Min(shortestPathUsingNewEdges, shortestPathToBeginingOfEdge + edge.Cost);
                    }

                    var shortestPathNotUsingNewEdges = shortestPath[nodeId, maxPathLen - 1];
                    int newShortestPath = Math.Min(shortestPathNotUsingNewEdges, shortestPathUsingNewEdges);
                    if (!pathChanged && newShortestPath != shortestPath[nodeId, maxPathLen - 1])
                    {
                        pathChanged = true;
                    }
                    shortestPath[nodeId, maxPathLen] = newShortestPath;
                }

                if (!pathChanged)
                    break;
            }

            if (pathChanged) // if path changed at the last iteration then negative cycle exists
            {
                throw new ArgumentException("Negative cycle is detected");
            }

            var minPaths = new int[graph.Nodes.Count];
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                minPaths[nodeId] = shortestPath[nodeId, maxPathLen];
            }

            return minPaths;
        }

        private static void InitWithInfiniteLength(int[,] shortestPath)
        {
            for (int i = 0; i < shortestPath.GetLength(0); i++)
            {
                for (int j = 0; j < shortestPath.GetLength(1); j++)
                {
                    shortestPath[i, j] = int.MaxValue;
                }
            }
        }

        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\g3.txt");

            var shortestPathInGraph = int.MaxValue;
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var shortestPath = BellmanFord(nodeId, graph);
                var shortestPathFromCurrentStartNode = shortestPath.Min();

                shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPathFromCurrentStartNode);
            }

            Console.WriteLine(shortestPathInGraph);
        }
    }
}
