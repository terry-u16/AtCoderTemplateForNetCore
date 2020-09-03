using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Numerics;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class LazySegmentTreeTest
    {
        struct MinInt : IMonoid<MinInt>
        {
            public MinInt Identity => new MinInt(1L << 50);
            public readonly long Value;

            public MinInt(long value) => Value = value;
            public MinInt Multiply(MinInt other) => Value < other.Value ? this : other;
        }

        struct Updater : IMonoidWithAct<MinInt, Updater>, IEquatable<Updater>
        {
            public Updater Identity => new Updater(long.MinValue, int.MinValue);

            public readonly long Value;
            public readonly int Generation;

            public Updater(long value, int generation)
            {
                Value = value;
                Generation = generation;
            }

            public MinInt Act(MinInt monoid) => new MinInt(Value);
            public Updater Multiply(Updater other) => Generation > other.Generation ? this : other;
            public bool Equals(Updater other) => Value == other.Value;
        }

        /// <summary>
        /// https://onlinejudge.u-aizu.ac.jp/problems/DSL_2_D
        /// </summary>
        [Fact]
        public void RangeUpdateQueryTest()
        {
            var n = 3;
            var segTree = new LazySegmentTree<MinInt, Updater>(Enumerable.Repeat((1L << 31) - 1, n).Select(i => new MinInt(i)).ToArray());

            segTree.Update(0, 2, new Updater(1, 0));
            segTree.Update(1, 3, new Updater(3, 1));
            segTree.Update(2, 3, new Updater(2, 2));
            Assert.Equal(1, segTree.Query(0, 1).Value);
            Assert.Equal(3, segTree.Query(1, 2).Value);
            Assert.Equal(2, segTree.Query(2, 3).Value);
        }
    }
}
