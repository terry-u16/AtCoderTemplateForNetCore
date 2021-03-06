﻿using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using AtCoderTemplateForNetCore.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
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
            Assert.Equal(gcd, NumericalAlgorithms.Lcm(a, b));
        }

        [Theory]
        [InlineData(2, 0, 0)]
        [InlineData(0, 1000000007, 0)]
        public void LcmWithZeroTest(long a, long b, long gcd)
        {
            Assert.Equal(gcd, NumericalAlgorithms.Lcm(a, b));
        }

        [Theory]
        [InlineData(-1, 3)]
        [InlineData(5, -10)]
        public void LcmThrowsExceptionTest(long a, long b)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => NumericalAlgorithms.Lcm(a, b));
        }
    }
}
