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
    public class WarshallFloydTest
    {
        [Theory]
        [InlineData("00-sample-00")]
        [InlineData("01-handmade-07")]
        [InlineData("after_contest_00")]
        public void ABC143ETest(string testCaseName)
        {
            const string basePath = @"..\..\..\TestCases\Graphs\WarshallFloydTest\ABC143E";
            var inputStream = new FileStream(Path.Join(basePath, "in", testCaseName), FileMode.Open, FileAccess.Read);
            var answerReader = new StreamReader(Path.Join(basePath, "out", testCaseName));

            var io = new IOManager(inputStream, new MemoryStream());

            var citiesCount = io.ReadInt();
            var roadsCount = io.ReadInt();
            var tankCapacity = io.ReadInt();
            
            var roadGraph = new WeightedGraph(citiesCount);
            for (int i = 0; i < roadsCount; i++)
            {
                var a = io.ReadInt() - 1;
                var b = io.ReadInt() - 1;
                var fuel = io.ReadInt();
                roadGraph.AddEdge(new WeightedEdge(a, b, fuel));
                roadGraph.AddEdge(new WeightedEdge(b, a, fuel));
            }

            var distanceWarshallFloyd = new WarshallFloyd<BasicNode, WeightedEdge>(roadGraph);
            var distances = distanceWarshallFloyd.GetDistances();

            // こっちの初期化手法はあまり使わないか……
            var refuelEdges = Enumerable.Range(0, citiesCount)
                                        .SelectMany(from => Enumerable.Range(0, citiesCount)
                                                                      .Select(to => new { from, to })
                                                                      .Where(pair => distances[pair.from, pair.to] <= tankCapacity)
                                                                      .Select(pair => new WeightedEdge(pair.from, pair.to)));
            var refuelGraph = new WeightedGraph(citiesCount, refuelEdges);
            var refuelWarshallFloyd = new WarshallFloyd<BasicNode, WeightedEdge>(refuelGraph);
            var refuelCounts = refuelWarshallFloyd.GetDistances();

            var queries = io.ReadInt();
            for (int q = 0; q < queries; q++)
            {
                var from = io.ReadInt() - 1;
                var to = io.ReadInt() - 1;
                var refuelCount = refuelCounts[from, to];
                var output = refuelCount < long.MaxValue ? (refuelCount - 1).ToString() : (-1).ToString();
                Assert.Equal(answerReader.ReadLine(), output);
            }
        }
    }
}
