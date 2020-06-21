﻿using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    [Collection("ModularCombination")]
    public class ModularTest
    {
        public ModularTest()
        {
            Modular.InitializeCombinationTable();
        }

        [Theory]
        [InlineData(1, 3, 1)]
        [InlineData(1000000010, 1000000007, 3)]
        [InlineData(-3, 1000000007, 1000000004)]
        [InlineData(2147483662032385532, 1000000007, 3)]
        public void InitializationTest(long value, int mod, int expected)
        {
            Modular.Mod = mod;
            var m = new Modular(value);
            Assert.Equal(mod, Modular.Mod);
            Assert.Equal(expected, m.Value);
            Assert.Equal(expected, (int)m);
            Assert.Equal(expected, (long)m);
            Assert.Equal($"{m.Value} (mod {Modular.Mod})", m.ToString());
            Modular.Mod = 1000000007;
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(1000000000, 10, 3)]
        public void AddTest(long a, long b, int expected)
        {
            var result = new Modular(a) + new Modular(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(2, 3, 1000000006)]
        public void SubtractTest(long a, long b, int expected)
        {
            var result = new Modular(a) - new Modular(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(10000, 100001, 9993)]
        public void MultiplicationTest(long a, long b, int expected)
        {
            var result = new Modular(a) * new Modular(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(6, 3, 2)]
        [InlineData(100, 10, 10)]
        [InlineData(1000000010, 100000001, 10)]
        public void DivisionTest(long a, long b, int expected)
        {
            var result = new Modular(a) / new Modular(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(3, 3, true)]
        [InlineData(2, 3, false)]
        [InlineData(1000000010, 3, true)]
        public void EqualsTest(long a, long b, bool expected)
        {
            var modA = new Modular(a);
            var modB = new Modular(b);
            Assert.Equal(expected, modA == modB);
            Assert.Equal(!expected, modA != modB);
            Assert.Equal(expected, modA.Equals(modB));
            Assert.Equal(expected, modA.Equals(modB as object));
        }

        [Theory]
        [InlineData(2, 3, false, true)]
        [InlineData(3, 3, false, false)]
        [InlineData(1000000010, 2, true, false)]
        public void CompareTest(long a, long b, bool greaterThan, bool smallerThan)
        {
            var modA = new Modular(a);
            var modB = new Modular(b);
            Assert.Equal(greaterThan, modA > modB);
            Assert.Equal(!greaterThan, modA <= modB);
            Assert.Equal(smallerThan, modA < modB);
            Assert.Equal(!smallerThan, modA >= modB);
        }

        [Theory]
        [InlineData(2, 3, 8)]
        [InlineData(10, 10, 999999937)]
        public void PowTest(long a, int n, int expected)
        {
            var modA = new Modular(a);
            var pow = Modular.Pow(modA, n);
            Assert.Equal(expected, pow.Value);
        }

        [Theory]
        [InlineData(3, 6)]
        [InlineData(10, 3628800)]
        [InlineData(15, 674358851)]
        public void FactorialTest(int n, int expected)
        {
            var fact = Modular.Factorial(n);
            Assert.Equal(expected, fact.Value);
        }

        [Theory]
        [InlineData(4, 2, 12)]
        [InlineData(10, 0, 1)]
        [InlineData(10, 1, 10)]
        [InlineData(10, 10, 3628800)]
        [InlineData(15, 15, 674358851)]
        [InlineData(10000, 15, 151968432)]
        public void PermutationTest(int n, int r, int expected)
        {
            var fact = Modular.Permutation(n, r);
            Assert.Equal(expected, fact.Value);
        }

        [Theory]
        [InlineData(4, 2, 6)]
        [InlineData(10, 0, 1)]
        [InlineData(10, 1, 10)]
        [InlineData(10, 10, 1)]
        [InlineData(15, 15, 1)]
        [InlineData(10000, 15, 21715928)]
        [InlineData(0, 0, 1)]
        public void CombinationTest(int n, int r, int expected)
        {
            var fact = Modular.Combination(n, r);
            Assert.Equal(expected, fact.Value);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 3)]
        [InlineData(6, 3, 56)]
        [InlineData(12, 5, 4368)]
        [InlineData(20, 10, 20030010)]
        [InlineData(30, 18, 648093714)]
        [InlineData(50, 14, 265331116)]
        public void CombinationWithRepititionTest(int n, int r, int expected)
        {
            var fact = Modular.CombinationWithRepetition(n, r);
            Assert.Equal(expected, fact.Value);
        }
    }
}
