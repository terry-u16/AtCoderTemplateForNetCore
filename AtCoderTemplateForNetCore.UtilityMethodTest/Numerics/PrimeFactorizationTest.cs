using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
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
            var result = NumericalAlgorithms.PrimeFactorization(n);
            var expected = new[] { (prime: 2, count: 2) };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PrimeFactorizationTest24()
        {
            var n = 24;
            var result = NumericalAlgorithms.PrimeFactorization(n);
            var expected = new[] { (prime: 2, count: 3), (prime: 3, count: 1) };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PrimeFactorizationTest13()
        {
            var n = 13;
            var result = NumericalAlgorithms.PrimeFactorization(n);
            var expected = new[] { (prime: 13, count: 1) };

            Assert.Equal(expected, result);
        }
    }
}
