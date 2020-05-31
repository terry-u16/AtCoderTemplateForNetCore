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
    public class BinarySearchTest
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(4, 6)]
        [InlineData(5, 6)]
        [InlineData(6, 7)]
        [InlineData(7, 9)]
        public void GetGreaterEqualIndexTest(int minValue, int expectedIndex)
        {
            var collection = new int[] { 1, 1, 2, 2, 3, 3, 5, 6, 6 };
            var index = collection.AsSpan().GetGreaterEqualIndex(minValue);

            Assert.Equal(expectedIndex, index);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 6)]
        [InlineData(4, 6)]
        [InlineData(5, 7)]
        [InlineData(6, 9)]
        [InlineData(7, 9)]
        public void GetGreaterThanIndexTest(int minValue, int expectedIndex)
        {
            var collection = new int[] { 1, 1, 2, 2, 3, 3, 5, 6, 6 };
            var index = collection.AsSpan().GetGreaterThanIndex(minValue);

            Assert.Equal(expectedIndex, index);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 5)]
        [InlineData(4, 5)]
        [InlineData(5, 6)]
        [InlineData(6, 8)]
        [InlineData(7, 8)]
        public void GetLessEqualIndexTest(int maxValue, int expectedIndex)
        {
            var collection = new int[] { 1, 1, 2, 2, 3, 3, 5, 6, 6 };
            var index = collection.AsSpan().GetLessEqualIndex(maxValue);

            Assert.Equal(expectedIndex, index);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(2, 1)]
        [InlineData(3, 3)]
        [InlineData(4, 5)]
        [InlineData(5, 5)]
        [InlineData(6, 6)]
        [InlineData(7, 8)]
        public void GetLessThanIndexTest(int maxValue, int expectedIndex)
        {
            var collection = new int[] { 1, 1, 2, 2, 3, 3, 5, 6, 6 };
            var index = collection.AsSpan().GetLessThanIndex(maxValue);

            Assert.Equal(expectedIndex, index);
        }

        [Fact]
        public void BisectionIntTest()
        {
            var answer = SearchExtensions.BoundaryBinarySearch(n => n * n >= 64, 100, 0);
            Assert.Equal(8, answer);
        }

        [Fact]
        public void BisectionDoubleTest()
        {
            var answer = SearchExtensions.Bisection(x => Math.Log(x), 1e-8, 1e18);
            Assert.InRange(Math.Abs(1 - answer), 0, 1e-9);
        }
    }
}
