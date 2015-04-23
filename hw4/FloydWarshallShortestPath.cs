using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;

namespace hw4
{
    class FloydWarshallShortestPath
    {
        public static int[,] Compute(Graph graph)
        {
            var shortestPathsCurrent = new int[graph.Nodes.Count, graph.Nodes.Count];
            var shortestPathsPrev = new int[graph.Nodes.Count, graph.Nodes.Count];

            Init0PathLengths(shortestPathsPrev, graph);
            for (int maxAllowedNodeIdx = 0; maxAllowedNodeIdx < graph.Nodes.Count; maxAllowedNodeIdx++) // note here that 0 permits node with idx = 0
            {
                for (int startNode = 0; startNode < graph.Nodes.Count; startNode++)
                {
                    for (int endNode = 0; endNode < graph.Nodes.Count; endNode++)
                    {
                        var minPathLenUsingNewlyAllowedNode = int.MaxValue;
                        var shortestPathToK = shortestPathsPrev[startNode, maxAllowedNodeIdx];
                        var shortestPathFromK = shortestPathsPrev[maxAllowedNodeIdx, endNode];

                        var shortestPathThorughK = (long)shortestPathToK + (long)shortestPathFromK;
                        if (shortestPathThorughK < minPathLenUsingNewlyAllowedNode)
                            minPathLenUsingNewlyAllowedNode = (int)shortestPathThorughK;

                        shortestPathsCurrent[startNode, endNode] = Math.Min(shortestPathsPrev[startNode, endNode], minPathLenUsingNewlyAllowedNode);
                    }
                }

                Swap(ref shortestPathsCurrent, ref shortestPathsPrev);
            }

            return shortestPathsPrev;
        }

        private static void Swap<T>(ref T p1, ref T p2)
        {
            var tmp = p1;
            p1 = p2;
            p2 = tmp;
        }

        private static void Init0PathLengths(int[,] shortestPath, Graph graph)
        {
            for (int i = 0; i < shortestPath.GetLength(0); i++)
            {
                for (int j = 0; j < shortestPath.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        shortestPath[i, j] = 0;
                    }
                    else
                    {
                        var directLinkMinLen = int.MaxValue;
                        foreach (var edge in graph.Nodes[i].Edges)
                        {
                            if (edge.Node1 != i) // if not originate from current node - skip it
                                continue;

                            if (edge.Node2 == j)
                            {
                                directLinkMinLen = Math.Min(directLinkMinLen, edge.Cost);
                            }
                        }

                        shortestPath[i, j] = directLinkMinLen;
                    }

                }
            }
        }
    }
}
