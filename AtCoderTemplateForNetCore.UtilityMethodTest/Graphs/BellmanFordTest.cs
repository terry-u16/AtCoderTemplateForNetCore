using System;
using System.IO;
using System.Linq;
using AtCoderTemplateForNetCore.Extensions;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
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
            ABC137Test_Core(testCaseName, (nodesCount, edgesCount, penalty, inputReader) =>
            {
                var graph = new WeightedGraph(nodesCount, Enumerable.Repeat(0, edgesCount).Select(_ =>
                {
                    var abc = inputReader.ReadIntArray();
                    var a = abc[0] - 1;
                    var b = abc[1] - 1;
                    var c = abc[2];
                    return new WeightedEdge(a, b, penalty - c);
                }));

                var bellmanFord = new BellmanFord<BasicNode, WeightedEdge>(graph);
                return bellmanFord;
            });
        }

        [Theory]
        [InlineData("b17")]
        [InlineData("after_contest_59")]
        public void ABC137Test_FromEdges(string testCaseName)
        {
            ABC137Test_Core(testCaseName, (nodesCount, edgesCount, penalty, inputReader) => 
                new BellmanFord<BasicNode, WeightedEdge>(
                    Enumerable.Repeat(0, edgesCount)
                            .Select(_ =>
                            {
                                var input = inputReader.ReadIntArray();
                                var a = input[0] - 1;
                                var b = input[1] - 1;
                                var c = input[2];
                                return new WeightedEdge(a, b, penalty - c);
                            }), nodesCount));
        }

        private void ABC137Test_Core(string testCaseName, Func<int, int, int, TextReader, BellmanFord<BasicNode, WeightedEdge>> buildBellmanFord)
        {
            const string basePath = @"..\..\..\TestCases\Graphs\BellmanFordTest\ABC137E";
            var inputReader = new StreamReader(Path.Join(basePath, "in", testCaseName));
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));
            var (nodesCount, edgesCount, penalty) = inputReader.ReadValue<int, int, int>();

            var bellmanFord = buildBellmanFord(nodesCount, edgesCount, penalty, inputReader);

            var result = bellmanFord.GetDistancesFrom(new BasicNode(0));
            var distances = result.Item1;
            var negativeCycleNodes = result.Item2;

            var output = (negativeCycleNodes[nodesCount - 1] ? -1 : Math.Max(-distances[nodesCount - 1], 0)).ToString();
            Assert.Equal(outputReader.ReadLine(), output);
        }
    }
}
