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
        public void ShrinkTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var shrinker = new CoordinateShrinker<int>(rawCoordinate);

            Assert.Equal(4, shrinker.Count);

            Assert.Equal(0, shrinker.Shrink(1));
            Assert.Equal(1, shrinker.Shrink(3));
            Assert.Equal(2, shrinker.Shrink(4));
            Assert.Equal(3, shrinker.Shrink(8));
        }

        [Fact]
        public void ExpandTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var shrinker = new CoordinateShrinker<int>(rawCoordinate);

            Assert.Equal(4, shrinker.Count);

            Assert.Equal(1, shrinker.Expand(0));
            Assert.Equal(3, shrinker.Expand(1));
            Assert.Equal(4, shrinker.Expand(2));
            Assert.Equal(8, shrinker.Expand(3));
        }

        [Fact]
        public void EnumerateTest()
        {
            var rawCoordinate = new[] { 3, 8, 1, 1, 4 };
            var shrinker = new CoordinateShrinker<int>(rawCoordinate);

            var expected = new[] { (0, 1), (1, 3), (2, 4), (3, 8) };
            Assert.Equal(expected, shrinker);
        }
    }
}
