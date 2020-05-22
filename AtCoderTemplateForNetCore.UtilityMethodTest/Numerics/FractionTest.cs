using AtCoderTemplateForNetCore.Numerics;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class FractionTest
    {
        [Theory]
        [InlineData(1, 2, 1, 2)]
        [InlineData(-1, 2, -1, 2)]
        [InlineData(1, -2, -1, 2)]
        [InlineData(3, -6, -1, 2)]
        [InlineData(-3, -6, 1, 2)]
        [InlineData(100, 0, 1, 0)]
        [InlineData(-100, 0, -1, 0)]
        [InlineData(0, 0, 0, 0)]
        public void InitializeTest(long numerator, long denominator, long expectedNumerator, long expectedDenominator)
        {
            var frac = new Fraction(numerator, denominator);
            Assert.Equal(expectedNumerator, frac.Numerator);
            Assert.Equal(expectedDenominator, frac.Denominator);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, 1, 1)]
        [InlineData(-1, 2, 1, 2, 0, 1)]
        [InlineData(1, 2, 1, 3, 5, 6)]
        [InlineData(1, 2, 1, 6, 2, 3)]
        [InlineData(100, 0, 1, 6, 1, 0)]
        [InlineData(-100, 0, 1, 0, 0, 0)]
        [InlineData(0, 0, 1, 1, 0, 0)]
        public void AddTest(long numeratorA, long denominatorA, long numeratorB, long denominatorB, long expectedNumerator, long expectedDenominator)
        {
            var fracA = new Fraction(numeratorA, denominatorA);
            var fracB = new Fraction(numeratorB, denominatorB);
            var added = fracA + fracB;
            Assert.Equal(expectedNumerator, added.Numerator);
            Assert.Equal(expectedDenominator, added.Denominator);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, 0, 1)]
        [InlineData(-1, 2, 1, 2, -1, 1)]
        [InlineData(1, 2, 1, 3, 1, 6)]
        [InlineData(1, 2, 1, 6, 1, 3)]
        [InlineData(100, 0, 1, 6, 1, 0)]
        [InlineData(100, 0, 1, 0, 0, 0)]
        [InlineData(-100, 0, 1, 0, -1, 0)]
        [InlineData(0, 0, 1, 1, 0, 0)]
        [InlineData(0, 1, 1, 0, -1, 0)]
        public void SubractTest(long numeratorA, long denominatorA, long numeratorB, long denominatorB, long expectedNumerator, long expectedDenominator)
        {
            var fracA = new Fraction(numeratorA, denominatorA);
            var fracB = new Fraction(numeratorB, denominatorB);
            var added = fracA - fracB;
            Assert.Equal(expectedNumerator, added.Numerator);
            Assert.Equal(expectedDenominator, added.Denominator);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, 1, 4)]
        [InlineData(-1, 2, 1, 2, -1, 4)]
        [InlineData(1, 2, 1, 3, 1, 6)]
        [InlineData(5, 2, 4, 25, 2, 5)]
        [InlineData(100, 99, -33, 100, -1, 3)]
        [InlineData(100, 0, 1, 6, 1, 0)]
        [InlineData(100, 0, -1, 0, -1, 0)]
        [InlineData(0, 0, 1, 1, 0, 0)]
        [InlineData(0, 1, 1, 0, 0, 0)]
        public void MultiplyTest(long numeratorA, long denominatorA, long numeratorB, long denominatorB, long expectedNumerator, long expectedDenominator)
        {
            var fracA = new Fraction(numeratorA, denominatorA);
            var fracB = new Fraction(numeratorB, denominatorB);
            var added = fracA * fracB;
            Assert.Equal(expectedNumerator, added.Numerator);
            Assert.Equal(expectedDenominator, added.Denominator);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, true)]
        [InlineData(-1, 2, 1, 2, false)]
        [InlineData(1, 2, 2, 4, true)]
        [InlineData(1, 2, -3, -6, true)]
        [InlineData(100, 0, 1, 0, true)]
        [InlineData(-100, 0, 1, 0, false)]
        [InlineData(0, 0, 1, 1, false)]
        [InlineData(0, 0, 0, 0, true)]
        [InlineData(0, 0, 1, 0, false)]
        [InlineData(0, 0, -1, 0, false)]
        public void EqualTest(long numeratorA, long denominatorA, long numeratorB, long denominatorB, bool expected)
        {
            var fracA = new Fraction(numeratorA, denominatorA);
            var fracB = new Fraction(numeratorB, denominatorB);
            Assert.Equal(expected, fracA == fracB);
            Assert.Equal(expected, fracA.Equals(fracB));
        }
    }
}
