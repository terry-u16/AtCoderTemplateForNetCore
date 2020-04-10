using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class LcmTest
    {
        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 3, 6)]
        [InlineData(6, 12, 12)]
        [InlineData(4, 9, 36)]
        [InlineData(109, 61, 6649)]
        [InlineData(12345678, 256803, 1056802382478)]
        public void LcmTest1(long a, long b, long gcd)
        {
            Assert.Equal(gcd, BasicAlgorithm.Lcm(a, b));
        }

        [Theory]
        [InlineData(-1, 3)]
        [InlineData(5, 0)]
        public void LcmThrowsExceptionTest(long a, long b)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BasicAlgorithm.Lcm(a, b));
        }
    }
}
