using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AtCoderTemplateForNetCore.Algorithms;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    public class UpdateWhenLargeSmallTest
    {
        [Theory]
        [InlineData(3, 5, 5)]
        [InlineData(0, 9, 9)]
        [InlineData(6, 2, 6)]
        public void UpdateWhenLargeTest(int first, int second, int expected)
        {
            AlgorithmHelpers.UpdateWhenLarge(ref first, second);
            Assert.Equal(expected, first);
        }

        [Theory]
        [InlineData(3, 5, 3)]
        [InlineData(0, 9, 0)]
        [InlineData(6, 2, 2)]
        public void UpdateWhenSmallTest(int first, int second, int expected)
        {
            AlgorithmHelpers.UpdateWhenSmall(ref first, second);
            Assert.Equal(expected, first);
        }

    }
}
