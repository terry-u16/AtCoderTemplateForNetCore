using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Numerics;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    [Collection("ModularCombination")]
    public class RerootingTest
    {
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
            var graph = Enumerable.Repeat(0, n).Select(_ => new List<int>()).ToArray();
            foreach (var input in inputs.Skip(1).Select(s => s.Split(' ').Select(s => int.Parse(s) - 1).ToArray()))
            {
                graph[input[0]].Add(input[1]);
                graph[input[1]].Add(input[0]);
            }

            var rerooting = new Rerooting<DPState>(graph);
            var result = rerooting.Solve().Select(r => r.Count.Value);

            var expected = new int[] { 40, 280, 840, 120, 120, 504, 72, 72 };
            Assert.Equal(expected, result);
        }
    }

    struct DPState : ITreeDpState<DPState>
    {
        public Modular Count { get; }
        public int Size { get; }

        public DPState Identity => new DPState(new Modular(1), 0);

        public DPState(Modular count, int size)
        {
            Count = count;
            Size = size;
        }

        public DPState AddRoot() => new DPState(Count, Size + 1);

        public DPState Multiply(DPState other)
        {
            var size = Size + other.Size;
            var count = Modular.Combination(size, Size) * Count * other.Count;
            return new DPState(count, size);
        }
    }
}
