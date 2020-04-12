using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class GetGreaterLessRangeTest
    {
        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 2, 1, 2, 4, 5)]
        [InlineData(10, 5, 1, 2, 3, 4, 5)]
        [InlineData(0, 0, 1, 2, 3, 4, 5)]
        public void GetRangeGreaterEqualTest(int minValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var index = collection.AsSpan().GetRangeGreaterEqual(minValue);
            Assert.Equal(expectedIndex, index.Start);
        }

        [Theory]
        [InlineData(3, 3, 1, 2, 3, 4, 5)]
        [InlineData(3, 2, 1, 2, 4, 5)]
        [InlineData(10, 5, 1, 2, 3, 4, 5)]
        [InlineData(0, 0, 1, 2, 3, 4, 5)]
        public void GetRangeSmallerEqualTest(int maxValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var index = collection.AsSpan().GetRangeSmallerEqual(maxValue);
            Assert.Equal(expectedIndex, index.End);
        }
    }
}
