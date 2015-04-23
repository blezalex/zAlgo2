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
        private static int[] BellmanFord(int sourceVertexIdx, Graph graph)
        {
            var shortestPathCurrent = new int[graph.Nodes.Count];
            var shortestPathPrev = new int[graph.Nodes.Count];

            InitWithInfiniteLength(shortestPathPrev);

            shortestPathPrev[sourceVertexIdx] = 0; // path from source vertex to source vertes with length 0 is empty path with 0 cost

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


                        var shortestPathToBeginingOfEdge = shortestPathPrev[edge.Node1];
                        if (shortestPathToBeginingOfEdge == int.MaxValue)
                            continue;

                        shortestPathUsingNewEdges = Math.Min(shortestPathUsingNewEdges, shortestPathToBeginingOfEdge + edge.Cost);
                    }

                    var shortestPathNotUsingNewEdges = shortestPathPrev[nodeId];
                    int newShortestPath = Math.Min(shortestPathNotUsingNewEdges, shortestPathUsingNewEdges);
                    if (!pathChanged && newShortestPath != shortestPathPrev[nodeId])
                    {
                        pathChanged = true;
                    }

                    shortestPathCurrent[nodeId] = newShortestPath;
                }

                if (!pathChanged)
                    break;

                Swap(ref shortestPathCurrent, ref shortestPathPrev);
            }

            if (pathChanged) // if path changed at the last iteration then negative cycle exists
            {
                throw new ArgumentException("Negative cycle is detected");
            }

            return shortestPathCurrent;
        }

        private static void Swap(ref int[] p1, ref int[] p2)
        {
            var tmp = p1;
            p1 = p2;
            p2 = tmp;
        }

        private static void InitWithInfiniteLength(int[] shortestPath)
        {
            for (int i = 0; i < shortestPath.GetLength(0); i++)
                shortestPath[i] = int.MaxValue;
            
        }

        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\g3.txt");
            
            var sw = new Stopwatch();  
            // 29.2 sec for naive bf * n
            // 16 sec bf * n    memory optimized
            sw.Start();
            
            var shortestPathInGraph = int.MaxValue;
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var shortestPath = BellmanFord(nodeId, graph);
                var shortestPathFromCurrentStartNode = shortestPath.Min();

                shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPathFromCurrentStartNode);
            }

            sw.Stop();

            Console.WriteLine("{0} {1}", shortestPathInGraph, sw.Elapsed);
        }
    }
}
