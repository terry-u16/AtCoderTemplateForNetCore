using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Numerics;
using AtCoderTemplateForNetCore.Graphs;
using AtCoderTemplateForNetCore.Graphs.Algorithms;
using Xunit;
using ModInt = AtCoderTemplateForNetCore.Numerics.StaticModInt<AtCoderTemplateForNetCore.Numerics.Mod1000000007>;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Graphs
{
    public class RerootingTest
    {
        public static ModCombination<Mod1000000007> Combination { get; private set; }

        static RerootingTest()
        {
            Combination = new ModCombination<Mod1000000007>();
        }

        [Fact]
        public void RerootingTestABC160ExampleF()
        {
            var inputs = @"8
1 2
2 3
3 4
3 5
3 6
6 7
6 8".Split(Environment.NewLine);

            var n = int.Parse(inputs[0]);
            var graph = new BasicGraph(n);
            foreach (var input in inputs.Skip(1).Select(s => s.Split(' ').Select(s => int.Parse(s) - 1).ToArray()))
            {
                graph.AddEdge(input[0], input[1]);
                graph.AddEdge(input[1], input[0]);
            }

            var rerooting = new Rerooting<BasicEdge, DPState>(graph);
            var result = rerooting.Solve().Select(r => r.Count.Value);

            var expected = new int[] { 40, 280, 840, 120, 120, 504, 72, 72 };
            Assert.Equal(expected, result);
        }
    }

    struct DPState : ITreeDpState<DPState>
    {
        public ModInt Count { get; }
        public int Size { get; }

        public DPState Identity => new DPState(ModInt.One, 0);

        public DPState(ModInt count, int size)
        {
            Count = count;
            Size = size;
        }

        public DPState AddRoot() => new DPState(Count, Size + 1);

        public DPState Merge(DPState other)
        {
            var size = Size + other.Size;
            var count = RerootingTest.Combination.Combination(size, Size) * Count * other.Count;
            return new DPState(count, size);
        }
    }
}
