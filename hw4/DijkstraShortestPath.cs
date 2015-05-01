using GraphLib;

namespace hw4
{
    class DijkstraShortestPath
    {
        public static int[] Compute(int sourceVertexIdx, Node[] nodes)
        {
            var shortestPaths = new int[nodes.Length];
            var shortestPathPrevHop = new int[nodes.Length];
            InitBaseCase(shortestPaths, shortestPathPrevHop);
            shortestPaths[sourceVertexIdx] = 0;// shortest paths from source to source = 0

            var heap = new DijkstraMinHeap(nodes.Length);
            heap.Add(new KeyValue<int, int>(0, sourceVertexIdx));

            for (int i = 0; i < nodes.Length; i++)
            {
                if (sourceVertexIdx != i)
                    heap.Add(new KeyValue<int, int>(int.MaxValue, i));
            }

            while (true)
            {
                var closestItem = heap.ExtractMin();
                if (closestItem == null)
                    break;

                var shortestPathToClosestItem = closestItem.Key;
                shortestPaths[closestItem.Value] = shortestPathToClosestItem;
                var closestNode = nodes[closestItem.Value];

                for (int edgeIdx = 0; edgeIdx < closestNode.Edges.Count; edgeIdx++)
                {
                    var edge = closestNode.Edges[edgeIdx];
                    if (edge.Node1 != closestItem.Value) // ignore incoming nodes
                        continue;

                    var edgeDst = heap.GetItemByValue(edge.Node2);
                    if (edgeDst != null) // path to that node is not found yet
                    {
                        var pathThoughThisEdge = shortestPathToClosestItem + edge.Cost;
                        if (edgeDst.Key > pathThoughThisEdge)
                            heap.DecreasePriority(edge.Node2, pathThoughThisEdge);
                    }
                }
            }

            return shortestPaths;
        }

        private static void InitBaseCase(int[] shortestPaths, int[] shortestPathPrevHop)
        {
            for (int i = 0; i < shortestPaths.Length; i++)
            {
                shortestPaths[i] = int.MaxValue;
                shortestPathPrevHop[i] = -1;
            }
        }
    }
}
