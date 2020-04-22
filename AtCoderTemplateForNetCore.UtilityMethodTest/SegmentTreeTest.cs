using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class SegmentTreeTest
    {
        [Fact]
        public void MinimumSegmentTreeTest()
        {
            var data = new int[] { 1, 2, 3, 4, 5 };
            var segmentTree = new SegmentTree<int>(data, (a, b) => Math.Min(a, b), int.MaxValue);
            Assert.Equal(1, segmentTree.Query(0..5));
            Assert.Equal(3, segmentTree.Query(2..5));

            segmentTree[4] = 0;
            Assert.Equal(0, segmentTree.Query(0..5));
            Assert.Equal(1, segmentTree.Query(0..4));

            segmentTree[1] = -5;
            Assert.Equal(-5, segmentTree.Query(0..5));
            Assert.Equal(-5, segmentTree.Query(1..5));
            Assert.Equal(0, segmentTree.Query(2..5));
            Assert.Equal(3, segmentTree.Query(2..4));
            Assert.Equal(3, segmentTree.Query(2, 4));

            var expected = new int[] { 1, -5, 3, 4, 0 };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], segmentTree[i]);
                Assert.Equal(expected[i], segmentTree.Data[i]);
            }
            Assert.Equal(expected, segmentTree);

            for (int i = 0; i < segmentTree.Length; i++)
            {
                segmentTree[i] = 10000;
            }
            Assert.Equal(10000, segmentTree.Query(0..segmentTree.Length));
        }
    }
}
