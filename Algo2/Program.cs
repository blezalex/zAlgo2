using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLib;

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

        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\edges.txt").Nodes;

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
