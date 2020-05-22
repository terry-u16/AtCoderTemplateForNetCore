using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
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
            var range = collection.AsSpan().GetRangeGreaterEqual(minValue);
            var (_, length) = range.GetOffsetAndLength(collection.Length);
            Assert.Equal(expectedIndex.., range);
            Assert.Equal(collection.Length - expectedIndex, length);
        }

        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 1, 1, 2, 4, 5)]
        [InlineData(10, 4, 1, 2, 3, 4, 5)]
        [InlineData(0, -1, 1, 2, 3, 4, 5)]
        public void GetRangeSmallerEqualTest(int maxValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var range = collection.AsSpan().GetRangeSmallerEqual(maxValue);
            var (_, length) = range.GetOffsetAndLength(collection.Length);
            Assert.Equal(0..(expectedIndex + 1), range);    // 0≦i＜n+1
            Assert.Equal(expectedIndex + 1, length);        // 0からnまでのn+1個
        }

        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 2, 1, 2, 4, 5)]
        [InlineData(10, 5, 1, 2, 3, 4, 5)]
        [InlineData(0, 0, 1, 2, 3, 4, 5)]
        public void GetRangeGreaterEqualSliceTest(int minValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var range = collection.AsSpan().GetRangeGreaterEqual(minValue);
            var sliced = collection[range];
            Assert.Equal(collection.AsSpan()[expectedIndex..].ToArray(), sliced.ToArray());
        }

        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 1, 1, 2, 4, 5)]
        [InlineData(10, 4, 1, 2, 3, 4, 5)]
        [InlineData(0, -1, 1, 2, 3, 4, 5)]
        public void GetRangeSmallerEqualSliceTest(int maxValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var range = collection.AsSpan().GetRangeSmallerEqual(maxValue);
            var sliced = collection[range];
            Assert.Equal(collection.AsSpan()[..(expectedIndex + 1)].ToArray(), sliced.ToArray());    // 0≦i＜n+1
        }

    }
}
