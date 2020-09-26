using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using AtCoderTemplateForNetCore.Questions;
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
            var io = new IOManager(new FileStream(Path.Join(basePath, "in", testCaseName), FileMode.Open, FileAccess.Read), new MemoryStream());
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));

            var nodesCount = io.ReadInt();
            var edgesCount = io.ReadInt();
            var graph = new WeightedGraph(nodesCount);
            var edges = new (int from, int to, long cost)[edgesCount];

            for (int i = 0; i < edgesCount; i++)
            {
                var a = io.ReadInt() - 1;
                var b = io.ReadInt() - 1;
                var cost = io.ReadInt();
                graph.AddEdge(a, b, cost);
                graph.AddEdge(b, a, cost);
                edges[i] = (a, b, cost);
            }

            var dijkstra = new Dijkstra(graph);

            var used = new bool[edgesCount];
            for (int node = 0; node < nodesCount; node++)
            {
                var distances = dijkstra.GetDistancesFrom(node);
                for (int edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
                {
                    var edge = edges[edgeIndex];
                    if (Math.Abs(distances[edge.from] - distances[edge.to]) == edge.cost)
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
