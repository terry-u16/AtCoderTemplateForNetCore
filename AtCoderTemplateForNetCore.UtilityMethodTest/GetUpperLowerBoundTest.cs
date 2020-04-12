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
    public class GetUpperLowerBoundTest
    {
        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 2, 1, 2, 4, 5)]
        [InlineData(10, 5, 1, 2, 3, 4, 5)]
        [InlineData(0, 0, 1, 2, 3, 4, 5)]
        public void GetLowerBoundTest(int minValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var index = collection.GetLowerBoundIndex(minValue);
            Assert.Equal(expectedIndex, index);
        }

        [Theory]
        [InlineData(3, 2, 1, 2, 3, 4, 5)]
        [InlineData(3, 1, 1, 2, 4, 5)]
        [InlineData(10, 4, 1, 2, 3, 4, 5)]
        [InlineData(0, -1, 1, 2, 3, 4, 5)]
        public void GetUpperBoundTest(int maxValue, int expectedIndex, params int[] collection)
        {
            Array.Sort(collection);
            var index = collection.GetUpperBoundIndex(maxValue);
            Assert.Equal(expectedIndex, index);
        }
    }
}
