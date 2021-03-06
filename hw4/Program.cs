﻿using System;
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
        static void Main(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\large.txt");

            var sw = new Stopwatch();
            sw.Start();


            var nodeIdsToConsiderAsStartingPoint = new List<int>(); // in order to be the shortest outgoing edge must be negative
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                if (graph.Nodes[nodeId].Edges.Any(e => e.Cost < 0))
                    nodeIdsToConsiderAsStartingPoint.Add(nodeId);
            }


            var shortestPathInGraph = int.MaxValue;
            var johnsonPathCalc = new JohnsonAllPairShortestPath(graph);
            
            var nodesWithNegativeEdges = graph.Nodes.Where(n => n.Edges.Any(e => e.Cost < 0));

            //for (int nodeId = 0; nodeId < graph.Nodes.Count - 1; nodeId++)
            Parallel.ForEach(nodeIdsToConsiderAsStartingPoint, nodeId =>
            {
                var shortestPath = johnsonPathCalc.Compute(nodeId);
                var shortestPathFromCurrentStartNode = shortestPath.Min();

                shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPathFromCurrentStartNode);
            });

            sw.Stop();
            Console.WriteLine("{0} {1}", shortestPathInGraph, sw.Elapsed);
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
            }

            graph.Nodes.Add(dummyNode);
            return dummyNodeId;
        }

        static void Main21(string[] args)
        {
            var graph = Graph.ReadFromFile(@"C:\Users\Alex\Desktop\algo2\g3.txt");
            
            var sw = new Stopwatch();  
            // 29.2 sec for naive bf * n
            // 16 sec bf * n    memory optimized
            // 7.9 FW   memory optimized
            // 7.36 FW count optimized
            // 1.24 Johnson
            sw.Start();

            var shortestPathInGraph = CalcPathWithNBF(graph);

            sw.Stop();

            Console.WriteLine("{0} {1}", shortestPathInGraph, sw.Elapsed);
        }

        private static int CalcPathWithFloydWarshal(Graph graph)
        {
            var shortestPath = FloydWarshallShortestPath.Compute(graph.Nodes);

            var shortestPathInGraph = int.MaxValue;
            for (int sourceNodeId = 0; sourceNodeId < graph.Nodes.Count; sourceNodeId++)
            {
                for (int destNodeId = 0; destNodeId < graph.Nodes.Count; destNodeId++)
                {
                    shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPath[sourceNodeId, destNodeId]);
                }   
            }

            return shortestPathInGraph;
        }

        private static int CalcPathWithNBF(Graph graph)
        {
            var shortestPathInGraph = int.MaxValue;
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var shortestPath = BellmandFordShortestPath.Compute(nodeId, graph);
                var shortestPathFromCurrentStartNode = shortestPath.Min();

                shortestPathInGraph = Math.Min(shortestPathInGraph, shortestPathFromCurrentStartNode);
            }

            return shortestPathInGraph;
        }
    }
}
