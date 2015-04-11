using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo2
{
    class Program
    {
        static void MainJobs(string[] args)
        {
            var jobs = JobSequencer.ReadJobs(@"C:\Users\Alex\Desktop\jobs.txt");
            var completionTimeSumDiff = JobSequencer.GetSumCompletionTime(JobSequencer.OrderByDiff(jobs));
            Console.WriteLine(completionTimeSumDiff);

            var completionTimeSumRatio = JobSequencer.GetSumCompletionTime(JobSequencer.OrderByRatio(jobs));
            Console.WriteLine(completionTimeSumRatio);
        }

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
        }

        static List<Node> ReadGraph(string filename)
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();

            using (var sr = File.OpenText(filename))
            {
                var infoLine = sr.ReadLine();
                var infoParts = infoLine.Split(' ');
                int nodeCnt = int.Parse(infoParts[0]);
                int edgeCnt = int.Parse(infoParts[1]);

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

            return nodes;
        }

        static void Main(string[] args)
        {
            var graph = ReadGraph(@"C:\Users\Alex\Desktop\edges.txt");

            var msp = BuildMspNaive(graph);

            var sum = msp.Sum(e => e.Cost);

            Console.WriteLine(sum);
        }

        private static List<Edge> BuildMspNaive(List<Node> graph)
        {
            int firstNode = 0;

            var msp = new List<Edge>(graph.Count);
            var connectedNodes = new Dictionary<int, bool>();
            connectedNodes[firstNode] = true;
            
            var frontier = new List<Edge>();
            frontier.AddRange(graph[firstNode].Edges);

            while (frontier.Count > 0)
            {
                var minEdge = GetMinEdge(frontier, connectedNodes);

                if (minEdge == null)
                    break;

                msp.Add(minEdge);

                var connectingNode = connectedNodes.ContainsKey(minEdge.Node1) ? minEdge.Node2 : minEdge.Node1;
                connectedNodes[connectingNode] = true;

                frontier.AddRange(graph[connectingNode].Edges);
            }


            return msp;
        }

        private static Edge GetMinEdge(List<Edge> frontier, Dictionary<int, bool> connectedNodes)
        {
            Edge minEdge = null; ;
            foreach (var edge in frontier)
            {
                if (connectedNodes.ContainsKey(edge.Node1) && connectedNodes.ContainsKey(edge.Node2))
                    continue;

                if (minEdge == null)
                {
                    minEdge = edge;
                    continue;
                }

                if (minEdge.Cost > edge.Cost)
                    minEdge = edge;
                    
            }

            return minEdge;
        }
    }
}
