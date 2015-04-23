using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;

namespace hw2
{
    class Program
    {
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

        static Int32[] GenerateMasks(int distance, int bits)
        {
            if (distance == 0)
                return new []{0};

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
            

            return masks.ToArray();
        }

        const int bits = 24;
        const int maxSearchDistance = 2;
        const int NoNode = -1;

        static void Main(string[] args)
        {
            var r = Part1();
            //Part2();
        }

        private static void Part2()
        {
            Int32[] nodes = ReadBinaryGraph(@"C:\Users\Alex\Desktop\algo2\clustering_big.txt");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // ******** Generate masks ***** /
            var masks = new Int32[maxSearchDistance + 1][];
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

            var graph = new UnionFind(nodes.Length);
            var numberOfClusters = nodes.Length - duplicateCount;

            while (true)
            {
                var pairOfDisjoinedPoints = SearchForNextClosestDisjointPairImplicit(nodeMap, masks, nodes, graph);

                if (pairOfDisjoinedPoints == null)
                    break; // no more close nodes found at maxDist

                graph.Union(pairOfDisjoinedPoints.Item1, pairOfDisjoinedPoints.Item2);

                numberOfClusters--;
            }

            Console.WriteLine(numberOfClusters);
            Console.WriteLine(sw.Elapsed);
        }


        static int currentDistance = 1;
        static int currentNodeIdx = 0;
        static int currentMaskIdx = 0;

        private static Tuple<int, int> SearchForNextClosestDisjointPairImplicit(int[] nodeMap, int[][] masks, int[] nodes, UnionFind graph)
        {
            while (currentDistance <= maxSearchDistance)
            {
                var masksForDistance = masks[currentDistance];
                while (currentNodeIdx < nodes.Length)
                {
                    var currentNode = nodes[currentNodeIdx];
                    if (nodeMap[currentNode] == currentNodeIdx) // otherwise dup and should be skipped
                    {
                        while (currentMaskIdx < masksForDistance.Length)
                        {
                            var mask = masksForDistance[currentMaskIdx++];

                            var lookupKey = currentNode ^ mask;

                            if (lookupKey < nodeMap.Length)
                            {
                                var mappedNode = nodeMap[lookupKey];
                                if (mappedNode != NoNode) // such node exist
                                {
                                    var setId1 = graph.Find(currentNodeIdx);
                                    var setId2 = graph.Find(mappedNode);

                                    if (setId1 != setId2) // is in the different set?
                                    {
                                        return Tuple.Create<int, int>(setId1, setId2);
                                    }
                                }
                            }
                        }
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
            var graphFromFile = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\clustering1.txt");

            List<Edge> graphEdges = graphFromFile.Edges;
            var unionFind = new UnionFind(graphFromFile.Nodes.Count);

            int numberOfClusters = graphFromFile.Nodes.Count;
            int k = 4; // desired max number of clusters

            var sortedEdgesForKraskal = graphEdges.OrderBy(e => e.Cost).GetEnumerator();

            while (numberOfClusters > k)
            {
                var pairOfDisjoinedPoints = SearchForNextClosestDisjointPair(sortedEdgesForKraskal, unionFind);

                unionFind.Union(pairOfDisjoinedPoints.Item1, pairOfDisjoinedPoints.Item2);
                numberOfClusters--;

                if (numberOfClusters <= k)
                    break;
            }

            return SearchForNextClosestDisjointPair(sortedEdgesForKraskal, unionFind).Item3.Cost;
        }

        private static Tuple<int, int, Edge> SearchForNextClosestDisjointPair(IEnumerator<Edge> sortedEdgesForKraskal, UnionFind clusters)
        {
            while (sortedEdgesForKraskal.MoveNext())
            {
                var currentEdge = sortedEdgesForKraskal.Current;

                var setId1 = clusters.Find(currentEdge.Node1);
                var setId2 = clusters.Find(currentEdge.Node2);
                if (setId1 != setId2) 
                {
                    return Tuple.Create(setId1, setId2, currentEdge);
                }

            }

            return null;
        }
    }
}
