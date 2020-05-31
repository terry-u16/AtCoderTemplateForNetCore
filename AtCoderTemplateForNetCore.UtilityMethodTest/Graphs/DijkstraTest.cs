using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AtCoderTemplateForNetCore.Extensions;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Graphs
{
    public class DijkstraTest
    {
        
        [Theory]
        [InlineData("sample_01.txt")]
        [InlineData("subtask_1_28.txt")]
        public void ABC051Test(string testCaseName)
        {
            const string basePath = @"..\..\..\TestCases\Graphs\DijkstraTest\ABC051D";
            var inputReader = new StreamReader(Path.Join(basePath, "in", testCaseName));
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));

            var (nodesCount, edgesCount) = inputReader.ReadValue<int, int>();
            var graph = new WeightedGraph(nodesCount);
            var edges = new WeightedEdge[edgesCount];

            for (int i = 0; i < edgesCount; i++)
            {
                var (a, b, cost) = inputReader.ReadValue<int, int, int>();
                a--;
                b--;
                graph.AddEdge(new WeightedEdge(a, b, cost));
                graph.AddEdge(new WeightedEdge(b, a, cost));
                edges[i] = new WeightedEdge(a, b, cost);
            }

            var dijkstra = new Dijkstra<BasicNode, WeightedEdge>(graph);

            var used = new bool[edgesCount];
            for (int node = 0; node < nodesCount; node++)
            {
                var distances = dijkstra.GetDistancesFrom(new BasicNode(node));
                for (int edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
                {
                    var edge = edges[edgeIndex];
                    if (Math.Abs(distances[edge.From.Index] - distances[edge.To.Index]) == edge.Weight)
                    {
                        used[edgeIndex] = true;
                    }
                }
            }

            var output = used.Count(b => !b).ToString();
            Assert.Equal(outputReader.ReadLine(), output);
        }
    }
}
