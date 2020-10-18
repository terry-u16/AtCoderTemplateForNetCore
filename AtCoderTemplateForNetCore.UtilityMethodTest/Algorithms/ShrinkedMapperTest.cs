using AtCoderTemplateForNetCore.Algorithms;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    public class ShrinkedMapperTest
    {
        [Fact]
        public void CompressTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var compressed = new CompressedCoordinate<int>(rawCoordinate);

            Assert.Equal(4, compressed.Count);

            Assert.Equal(1, compressed[0]);
            Assert.Equal(3, compressed[1]);
            Assert.Equal(0, compressed[2]);
            Assert.Equal(0, compressed[3]);
            Assert.Equal(2, compressed[4]);
        }

        [Fact]
        public void ExpandTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var shrinker = new CompressedCoordinate<int>(rawCoordinate);

            Assert.Equal(4, shrinker.Count);

            Assert.Equal(1, shrinker.Expand(0));
            Assert.Equal(3, shrinker.Expand(1));
            Assert.Equal(4, shrinker.Expand(2));
            Assert.Equal(8, shrinker.Expand(3));
        }
    }
}
