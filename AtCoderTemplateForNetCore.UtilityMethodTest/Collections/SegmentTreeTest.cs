using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using AtCoderTemplateForNetCore.Numerics;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class SegmentTreeTest
    {
        struct MinInt : IMonoid<MinInt>
        {
            public int Value { get; }

            public MinInt(int value)
            {
                Value = value;
            }

            public MinInt Identity => new MinInt(int.MaxValue);

            public MinInt Merge(MinInt other) => new MinInt(Math.Min(Value, other.Value));

            public static implicit operator int(MinInt min) => min.Value;
        }

        private readonly SegmentTree<MinInt> _defaultSegmentTree;

        public SegmentTreeTest()
        {
            var data = new int[] { 1, 2, 3, 4, 5 }.Select(i => new MinInt(i)).ToArray();
            _defaultSegmentTree = new SegmentTree<MinInt>(data);
        }

        [Fact]
        public void MinimumSegmentTreeTest()
        {
            var data = new int[] { 1, 2, 3, 4, 5 }.Select(i => new MinInt(i)).ToArray();
            var segmentTree = new SegmentTree<MinInt>(data);
            Assert.Equal(1, segmentTree.Query(0..5));
            Assert.Equal(3, segmentTree.Query(2..5));

            segmentTree[4] = new MinInt(0);
            Assert.Equal(0, segmentTree.Query(0..5));
            Assert.Equal(1, segmentTree.Query(0..4));

            segmentTree[^4] = new MinInt(-5);
            Assert.Equal(-5, segmentTree.Query(0..5));
            Assert.Equal(-5, segmentTree.Query(1..5));
            Assert.Equal(0, segmentTree.Query(2..5));
            Assert.Equal(3, segmentTree.Query(2..4));
            Assert.Equal(3, segmentTree.Query(2, 4));

            var expected = new int[] { 1, -5, 3, 4, 0 };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], segmentTree[i]);
            }

            for (int i = 0; i < segmentTree.Length; i++)
            {
                segmentTree[i] = new MinInt(10000);
            }
            Assert.Equal(10000, segmentTree.Query(0..segmentTree.Length));
        }

        [Fact]
        public void SegmentTreeQueryThrowsExceptionTest()
        {
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(-1, 2));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(0, -1));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(0, 6));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(5, 6));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(3, 2));

            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(-1..2));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(0..-1));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(0..6));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(5..6));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(3..2));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(3..^3));
            Assert.ThrowsAny<ArgumentException>(() => _defaultSegmentTree.Query(^0..^1));
        }

        [Fact]
        public void SegmentTreeIndexerThrowsExceptionTest()
        {
            Assert.ThrowsAny<IndexOutOfRangeException>(() => _defaultSegmentTree[-1] = new MinInt(4));
            Assert.ThrowsAny<IndexOutOfRangeException>(() => _defaultSegmentTree[5] = new MinInt(4));

            Assert.ThrowsAny<IndexOutOfRangeException>(() => _defaultSegmentTree[-1]);
            Assert.ThrowsAny<IndexOutOfRangeException>(() => _defaultSegmentTree[5]);
        }
    }
}
