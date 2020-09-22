using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using AtCoderTemplateForNetCore.Questions;
using Xunit;
using Xunit.Abstractions;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Graphs
{
    // 最短距離を求めるならBFSだけどテストということで……
    public class DfsTest
    {
        private readonly ITestOutputHelper _logger;

        public DfsTest(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Theory]
        [InlineData("sample1.txt")]
        [InlineData("sample2.txt")]
        [InlineData("sample3.txt")]
        [InlineData("sample4.txt")]
        [InlineData("sample5.txt")]
        public void ATP001ATest(string testCaseName)
        {
            const string basePath = @"..\..\..\TestCases\Graphs\DfsTest\ATP001A";
            var io = new IOManager(new FileStream(Path.Join(basePath, "in", testCaseName), FileMode.Open, FileAccess.Read), new MemoryStream());
            var outputReader = new StreamReader(Path.Join(basePath, "out", testCaseName));

            var height = io.ReadInt();
            var width = io.ReadInt();
            var map = new char[height][];
            for (int i = 0; i < height; i++)
            {
                map[i] = io.ReadString().ToCharArray();
            }

            var maze = new Maze(height, width, map);
            var solver = new MazeSolver(maze, _logger);
            var distance = solver.Search(maze.Start);
            Assert.Equal(outputReader.ReadLine(), distance.ToString());
        }

        class Maze : GridGraph
        {
            public char[][] Map { get; }
            public GridNode Start { get; }
            public GridNode Goal { get; }

            public Maze(int height, int width, char[][] map) : base(height, width)
            {
                Map = map;

                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        var c = map[row][column];
                        if (c == 's')
                        {
                            Start = new GridNode(row, column, width);
                        }
                        else if (c == 'g')
                        {
                            Goal = new GridNode(row, column, width);
                        }
                    }
                }
            }

            protected override bool CanEnter(GridNode node) => base.CanEnter(node) && Map[node.Row][node.Column] != '#';
        }

        class MazeSolver : DfsBase<Maze, GridNode, GridEdge, int>
        {
            readonly int[,] _distances;
            readonly ITestOutputHelper _logger;
            const int Inf = 1 << 28;

            public MazeSolver(Maze graph, ITestOutputHelper logger) : base(graph)
            {
                _logger = logger;
                _distances = new int[_graph.Height, _graph.Width];
            }

            protected override int GetResult()
            {
                var (row, column) = _graph.Goal;
                return _distances[row, column] < Inf ? _distances[row, column] : -1;
            }

            protected override void Initialize(GridNode startNode)
            {
                for (int row = 0; row < _graph.Height; row++)
                {
                    for (int column = 0; column < _graph.Width; column++)
                    {
                        _distances[row, column] = Inf;
                    }
                }
                _distances[_graph.Start.Row, _graph.Start.Column] = 0;
            }

            protected override void OnPostordering(GridNode current, GridNode previous, bool isFirstNode)
            {
                _logger.WriteLine($"[Leaving]({current.Row}, {current.Column})");
            }

            protected override void OnPreordering(GridNode current, GridNode previous, bool isFirstNode)
            {
                _logger.WriteLine($"[Entering]({current.Row}, {current.Column})");
                if (!isFirstNode)
                {
                    _distances[current.Row, current.Column] = _distances[previous.Row, previous.Column] + 1;

                    if (current == _graph.Goal)
                    {
                        _completed = true;
                        _logger.WriteLine("Completed!");
                    }
                }
            }
        }
    }
}
