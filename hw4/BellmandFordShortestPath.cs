using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;

namespace hw4
{
    class BellmandFordShortestPath
    {
        public static int[] Compute(int sourceVertexIdx, Graph graph)
        {
            var nodes = graph.Nodes.ToArray();

            var shortestPathCurrent = new int[nodes.Length];
            var shortestPathPrev = new int[nodes.Length];

            InitWithInfiniteLength(shortestPathPrev);

            shortestPathPrev[sourceVertexIdx] = 0; // path from source vertex to source vertes with length 0 is empty path with 0 cost

            bool pathChanged = true;
            int maxPathLen;

            for (maxPathLen = 1; maxPathLen <= nodes.Length; maxPathLen++)
            {
                pathChanged = false;

                for (int nodeId = 0; nodeId < nodes.Length; nodeId++)
                {
                    var shortestPathUsingNewEdges = int.MaxValue;
                    var node = nodes[nodeId];
                    for (int edgeId = 0; edgeId < node.Edges.Count; edgeId++)
                    {
                        var edge = node.Edges[edgeId];

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

        private static void Swap<T>(ref T p1, ref T p2)
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
    }
}
