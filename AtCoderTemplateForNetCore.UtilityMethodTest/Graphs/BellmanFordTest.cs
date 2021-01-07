using System;
using System.IO;
using System.Linq;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using AtCoderTemplateForNetCore.Problems;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Graphs
{
    public class BellmanFordTest
    {
        [Theory]
        [InlineData("b17")]
        [InlineData("after_contest_59")]
        public void ABC137Test_FromGraph(string testCaseName)
        {
            ABC137Test_Core(testCaseName, (nodesCount, edgesCount, penalty, io) =>
            {
                var graph = new WeightedGraph(nodesCount);

                for (int i = 0; i < edgesCount; i++)
                {
                    var abc = io.ReadIntArray(3);
                    var a = abc[0] - 1;
                    var b = abc[1] - 1;
                    var c = abc[2];
                    graph.AddEdge(a, b, penalty - c);
                }

                var bellmanFord = new BellmanFord(graph);
                return bellmanFord;
            });
        }

        [Theory]
        [InlineData("b17")]
        [InlineData("after_contest_59")]
        public void ABC137Test_FromEdges(string testCaseName)
        {
            ABC137Test_Core(testCaseName, (nodesCount, edgesCount, penalty, io) =>
            {
                var graph = new WeightedGraph(nodesCount);
                var bellmanFord = new BellmanFord(nodesCount);

                for (int i = 0; i < edgesCount; i++)
                {
                    var abc = io.ReadIntArray(3);
                    var a = abc[0] - 1;
                    var b = abc[1] - 1;
                    var c = abc[2];
                    bellmanFord.AddEdge(a, b, penalty - c);
                }

                return bellmanFord;
            });
        }

        private void ABC137Test_Core(string testCaseName, Func<int, int, int, IOManager, BellmanFord> buildBellmanFord)
        {
            const string basePath = @"..\..\..\TestCases\Graphs\BellmanFordTest\ABC137E";
            var io = new IOManager(new FileStream(Path.Join(basePath, "in", testCaseName), FileMode.Open, FileAccess.Read), new MemoryStream());
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));
            
            var nodesCount = io.ReadInt();
            var edgesCount = io.ReadInt();
            var penalty = io.ReadInt();

            var bellmanFord = buildBellmanFord(nodesCount, edgesCount, penalty, io);

            var (distances, negativeCycleNodes) = bellmanFord.GetDistancesFrom(0);

            var output = (negativeCycleNodes[nodesCount - 1] ? -1 : Math.Max(-distances[nodesCount - 1], 0)).ToString();
            Assert.Equal(outputReader.ReadLine(), output);
        }
    }
}
