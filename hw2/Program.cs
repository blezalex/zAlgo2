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

        static Int32[] ReadBinaryGraph(string filename)
        {
            using (var sr = File.OpenText(filename))
            {
                var infoLine = sr.ReadLine();
                var infoParts = infoLine.Split(' ');
                int nodeCnt = int.Parse(infoParts[0]);

                var nodes = new Int32[nodeCnt];

                string line;
                int nodeIdx = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    Int32 nodeValue = 0;

                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            continue;
                        }

                        nodeValue = nodeValue << 1;

                        if (line[i] == '1')
                        {
                            nodeValue |= 1;
                        }
                    }

                    nodes[nodeIdx++] = nodeValue;
                }
                return nodes;
            }            
        }


        

        static List<Int32> GenerateMasks(int distance, int bits)
        {
            if (distance == 0)
                return new List<Int32> {0};

            List<Int32> masks = new List<Int32>();

     
            var smallerMasks = GenerateMasks(distance - 1, bits);

            foreach (var mask in smallerMasks)
            {
                for (var i = 0; i < bits; i++)
                {
                    if ((mask & (1 << i)) == 0)
                    {
                        var newMask = mask | 1 << i;
                        if (!masks.Contains(newMask))
                        {
                            masks.Add(newMask);
                        }
                    }
                }
            }
            

            return masks;
        }

        const int bits = 24;
        const int maxSearchDistance = 2;
        const int NoNode = -1;

        static void Main(string[] args)
        {
            Int32[] nodes = ReadBinaryGraph(@"C:\Users\Alex\Desktop\clustering_big.txt");
            
            // ******** Generate masks ***** /
            var masks = new List<Int32>[maxSearchDistance + 1];
            for (var distance = 1; distance <= maxSearchDistance; distance++)
            {
                masks[distance] = GenerateMasks(distance, bits);
            }


            // ******** Generate map ******* /
            var nodeMapLen = nodes.Max() + 1;
            int duplicateCount = 0;
            var nodeMap = new Int32[nodeMapLen];
            for (int i = 0; i < nodeMapLen; i++)
            {
                nodeMap[i] = NoNode;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];

                if (nodeMap[node] != NoNode)
                    ++duplicateCount;

                nodeMap[node] = i;
            }

            var graph = new Node[nodes.Length];
            for (int i = 0; i < graph.Length; i++)
            {
                graph[i] = new Node();
            }

            var numberOfClusters = nodes.Length - duplicateCount;

            while (true)
            {
                var pairOfDisjoinedPoints = SearchForNextClosestDisjointPairImplicit(nodeMap, masks, nodes, graph);

                if (pairOfDisjoinedPoints == null)
                    break; // no more close nodes found at maxDist

                Join(pairOfDisjoinedPoints, graph);

                numberOfClusters--;
            }

            Console.WriteLine(numberOfClusters);
            Console.WriteLine(dupCnt);
        }


        static int currentDistance = 1;
        static int currentNodeIdx = 0;
        static int currentMaskIdx = 0;

        static int dupCnt = 0;

        static int[] edgesCnt = new Int32[maxSearchDistance + 1];

        private static Tuple<Node, Node, Edge> SearchForNextClosestDisjointPairImplicit(Int32[] nodeMap, IList<List<int>> masks, IList<Int32> nodes, IList<Node> graph)
        {
            
            while (currentDistance <= maxSearchDistance)
            {
                while (currentNodeIdx < nodes.Count)
                {
                    if (nodeMap[nodes[currentNodeIdx]] == currentNodeIdx) // otherwise dup and should be skipped
                    {
                        while (currentMaskIdx < masks[currentDistance].Count)
                        {
                            var mask = masks[currentDistance][currentMaskIdx++];

                            var lookupKey = nodes[currentNodeIdx] ^ mask;

                            if (lookupKey < nodeMap.Length)
                            {
                                var mappedNode = nodeMap[lookupKey];
                                if (mappedNode != NoNode) // such node exist
                                {
                                    var node1 = graph[currentNodeIdx];
                                    var node2 = graph[mappedNode];

                                    if (node1.Leader != node2.Leader) // is in the different set?
                                    {
                                        edgesCnt[currentDistance]++;
                                        return Tuple.Create<Node, Node, Edge>(node1, node2, null);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        dupCnt++;
                    }

                    currentMaskIdx = 0;
                    currentNodeIdx++;
                }

                currentNodeIdx = 0;
                currentDistance++;
            }

            return null;
        }

        private static int Part1()
        {
            var graphFromFile = ReadGraph(@"C:\Users\Alex\Desktop\clustering1.txt");

            List<Node> graph = graphFromFile.Nodes;
            List<Edge> graphEdges = graphFromFile.Edges;

            int numberOfClusters = graph.Count;
            int k = 4; // desired max number of clusters

            var sortedEdgesForKraskal = graphEdges.OrderBy(e => e.Cost).GetEnumerator();

            while (numberOfClusters > k)
            {
                var pairOfDisjoinedPoints = SearchForNextClosestDisjointPair(sortedEdgesForKraskal, graph);

                Join(pairOfDisjoinedPoints, graph);

                numberOfClusters--;

                // var countOfDistinctLeaders = graph.GroupBy(n => n.Leader).Count();

                if (numberOfClusters <= k)
                    break;
            }

            return SearchForNextClosestDisjointPair(sortedEdgesForKraskal, graph).Item3.Cost;
        }

        private static void Join(Tuple<Node, Node, Edge> pairOfDisjoinedPoints, IList<Node> graph)
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
