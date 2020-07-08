using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class GcdTest
    {
        [Theory]
        [InlineData(2, 3, 1)]
        [InlineData(2, 2, 2)]
        [InlineData(6, 3, 3)]
        [InlineData(18, 4, 2)]
        [InlineData(109, 61, 1)]
        [InlineData(12345678, 256803, 3)]
        public void GcdTest1(long a, long b, long gcd)
        {
            Assert.Equal(gcd, NumericalAlgorithms.Gcd(a, b));
        }

        [Theory]
        [InlineData(2, 0, 2)]
        [InlineData(0, 1000000007, 1000000007)]
        [InlineData(0, 0, 0)]
        public void GcdWithZeroTest(long a, long b, long gcd)
        {
            Assert.Equal(gcd, NumericalAlgorithms.Gcd(a, b));
        }

        [Theory]
        [InlineData(-1, 3)]
        [InlineData(5, -10)]
        public void GcdThrowsExceptionTest(long a, long b)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => NumericalAlgorithms.Gcd(a, b));
        }
    }
}
