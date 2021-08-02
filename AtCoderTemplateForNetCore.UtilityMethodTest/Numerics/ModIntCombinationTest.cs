using Xunit;
using AtCoderTemplateForNetCore.Numerics;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class ModIntCombinationTest
    {
        readonly ModCombination<Mod1000000007> _combination;

        public ModIntCombinationTest()
        {
            _combination = new ModCombination<Mod1000000007>();
        }

        [Theory]
        [InlineData(3, 6)]
        [InlineData(10, 3628800)]
        [InlineData(15, 674358851)]
        public void FactorialTest(int n, int expected)
        {
            var fact = _combination.Factorial(n);
            Assert.Equal(expected, fact.Value);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(15)]
        public void InvFactorialTest(int n)
        {
            var fact = _combination.Factorial(n);
            var invFact = _combination.InvFactorial(n);
            var prod = fact * invFact;
            Assert.Equal(1, prod.Value);
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
            var fact = _combination.Permutation(n, r);
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
            var fact = _combination.Combination(n, r);
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
            var fact = _combination.CombinationWithRepetition(n, r);
            Assert.Equal(expected, fact.Value);
        }
    }
}
