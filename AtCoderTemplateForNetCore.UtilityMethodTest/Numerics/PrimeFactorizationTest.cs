using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using AtCoderTemplateForNetCore.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class PrimeFactorizationTest
    {
        [Fact]
        public void PrimeFactorizationTest4()
        {
            var n = 4;
            var result = NumericalAlgorithms.PrimeFactorize(n);
            var expected = new (long, int)[] { (2, 2) };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PrimeFactorizationTest24()
        {
            var n = 24;
            var result = NumericalAlgorithms.PrimeFactorize(n);
            var expected = new (long, int)[] { (2, 3), (3, 1) };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PrimeFactorizationTest13()
        {
            var n = 13;
            var result = NumericalAlgorithms.PrimeFactorize(n);
            var expected = new (long, int)[] { (13, 1) };

            Assert.Equal(expected, result);
        }
    }
}
