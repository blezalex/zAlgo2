using GraphLib;

namespace hw4
{
    public class JohnsonAllPairShortestPath
    {
        private readonly Node[] nodes;

        private readonly int[] shortestPathForReweight;

        public JohnsonAllPairShortestPath(Graph graph)
        {
            nodes = graph.Nodes.ToArray();

            var dummyNodeId = AddDummyNode(graph);

            shortestPathForReweight = BellmandFordShortestPath.Compute(dummyNodeId, graph);

            Reweight(graph, shortestPathForReweight);
        }

        public int[] Compute(int sourceVertexIdx)
        {
            var shortestPath = DijkstraShortestPath.Compute(sourceVertexIdx, nodes);
            RestorePathLengths(shortestPath, shortestPathForReweight, sourceVertexIdx);

            return shortestPath;
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
                graph.Edges.Add(edge);
            }

            graph.Nodes.Add(dummyNode);
            return dummyNodeId;
        }

        private static void Reweight(Graph graph, int[] shortestPathForReweight)
        {
            foreach (var edge in graph.Edges)
            {
                var pu = shortestPathForReweight[edge.Node1];
                var pv = shortestPathForReweight[edge.Node2];

                edge.Cost += pu - pv;
            }
        }

        private static void RestorePathLengths(int[] shortestPath, int[] shortestPathForReweight, int sourceNodeId)
        {
            var pv = shortestPathForReweight[sourceNodeId];

            for (int dstIdx = 0; dstIdx < shortestPath.Length; dstIdx++)
            {
                var pu = shortestPathForReweight[dstIdx];

                shortestPath[dstIdx] += pv - pu;
            }
        }
    }
}
