using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using AtCoderTemplateForNetCore.Questions;
using Xunit;
using Xunit.Abstractions;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Graphs
{
    public class BfsTest
    {
        private readonly ITestOutputHelper _logger;

        public BfsTest(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Theory]
        [InlineData("sample_01.txt")]
        [InlineData("sample_02.txt")]
        public void ABC168DTest(string testCaseName)
        {
            // https://atcoder.jp/contests/abc168/submissions/13756313
            const string basePath = @"..\..\..\TestCases\Graphs\BfsTest\ABC168D";
            var io = new IOManager(new FileStream(Path.Join(basePath, "in", testCaseName), FileMode.Open, FileAccess.Read), new MemoryStream());
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));

            var rooms = io.ReadInt();
            var paths = io.ReadInt();

            var dungeon = new BasicGraph(rooms);
            for (int i = 0; i < paths; i++)
            {
                var a = io.ReadInt() - 1;
                var b = io.ReadInt() - 1;
                dungeon.AddEdge(new BasicEdge(a, b));
                dungeon.AddEdge(new BasicEdge(b, a));
            }

            var solver = new BfsSolver(dungeon, _logger);
            var previous = solver.Search(new BasicNode(0));

            _ = outputReader.ReadLine();    // Yes

            for (int i = 1; i < previous.Length; i++)
            {
                Assert.Equal(outputReader.ReadLine(), (previous[i] + 1).ToString());
            }
        }
    }

    class BfsSolver : BfsBase<BasicGraph, BasicNode, BasicEdge, int[]>
    {
        readonly int[] _previous;
        readonly ITestOutputHelper _logger;

        public BfsSolver(BasicGraph graph, ITestOutputHelper logger) : base(graph)
        {
            _previous = new int[graph.NodeCount];
            _logger = logger;
        }

        protected override int[] GetResult()
        {
            return _previous;
        }

        protected override void Initialize(BasicNode startNode)
        {
        }

        protected override void OnPreordering(BasicNode current, BasicNode previous, bool isFirstNode)
        {
            _logger.WriteLine($"[Entering]room{current}");

            if (!isFirstNode)
            {
                _previous[current.Index] = previous.Index;
            }
        }
    }
}
