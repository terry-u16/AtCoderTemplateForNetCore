using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class PermutationTest
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(3, 6)]
        [InlineData(6, 720)]
        [InlineData(15, 1307674368000)]
        public void FactorialCountTest(int n, long expected)
        {
            var result = BasicAlgorithm.Factorial(n);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(6, 3, 120)]
        [InlineData(12, 5, 95040)]
        [InlineData(20, 10, 670442572800)]
        public void PermutationCountTest(int n, int r, long expected)
        {
            var result = BasicAlgorithm.Permutation(n, r);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 1)]
        [InlineData(6, 3, 20)]
        [InlineData(12, 5, 792)]
        [InlineData(20, 10, 184756)]
        [InlineData(50, 20, 47129212243960)]
        [InlineData(50, 30, 47129212243960)]
        public void CombinationCountTest(int n, int r, long expected)
        {
            var result = BasicAlgorithm.Combination(n, r);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 3)]
        [InlineData(6, 3, 56)]
        [InlineData(12, 5, 4368)]
        [InlineData(20, 10, 20030010)]
        [InlineData(30, 18, 4568648125690)]
        [InlineData(50, 14, 37387265592825)]
        public void CombinationWithRepetitionCountTest(int n, int r, long expected)
        {
            var result = BasicAlgorithm.CombinationWithRepetition(n, r);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Permutation1Test()
        {
            var input = Enumerable.Range(1, 1);
            var expected = new[] { new int[] { 1 } };

            var actual = BasicAlgorithm.GetPermutations(input, false);

            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2Test()
        {
            var input = Enumerable.Range(1, 2);
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2ReversedTest()
        {
            var input = Enumerable.Range(1, 2).Reverse();
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2WithDuplicationTest()
        {
            var input = new int[] { 1, 1 };
            var expected = new[] { new int[] { 1, 1 } };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation3Test()
        {
            var input = Enumerable.Range(1, 3);
            var expected = new[] { 
                new int[] { 1, 2, 3 },
                new int[] { 1, 3, 2 },
                new int[] { 2, 1, 3 },
                new int[] { 2, 3, 1 },
                new int[] { 3, 1, 2 },
                new int[] { 3, 2, 1 }
            };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation3WithDuplicationTest()
        {
            var input = new int[] { 1, 2, 2 };
            var expected = new[] {
                new int[] { 1, 2, 2 },
                new int[] { 2, 1, 2 },
                new int[] { 2, 2, 1 }
            };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationEmptyTest()
        {
            var input = Enumerable.Empty<int>();
            var expected = new [] { new int[] { } };

            var actual = BasicAlgorithm.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationNullThrowsExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => BasicAlgorithm.GetPermutations(default(IEnumerable<int>), false).ToArray());            
        }

        private static void AssertSequentialEqual(int[][] expected, IEnumerable<ReadOnlyMemory<int>> actual)
        {
            foreach (var (exp, act) in expected.Zip(actual))
            {
                var actSpan = act.Span;
                Assert.Equal(exp.Length, actSpan.Length);
                for (int i = 0; i < exp.Length; i++)
                {
                    Assert.Equal(exp[i], actSpan[i]);
                }
            }
        }
    }
}
