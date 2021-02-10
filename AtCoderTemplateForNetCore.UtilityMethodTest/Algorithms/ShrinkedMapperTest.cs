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

            Assert.Equal(4, compressed.UniqueCount);

            Assert.Equal(1, compressed.Compressed[0]);
            Assert.Equal(3, compressed.Compressed[1]);
            Assert.Equal(0, compressed.Compressed[2]);
            Assert.Equal(0, compressed.Compressed[3]);
            Assert.Equal(2, compressed.Compressed[4]);
        }

        [Fact]
        public void ExpandTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var shrinker = new CompressedCoordinate<int>(rawCoordinate);

            Assert.Equal(4, shrinker.UniqueCount);

            Assert.Equal(1, shrinker.Invert(0));
            Assert.Equal(3, shrinker.Invert(1));
            Assert.Equal(4, shrinker.Invert(2));
            Assert.Equal(8, shrinker.Invert(3));
        }
    }
}
