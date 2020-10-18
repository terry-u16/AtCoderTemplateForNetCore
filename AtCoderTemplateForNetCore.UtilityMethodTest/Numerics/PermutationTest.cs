using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Collections;
using AtCoderTemplateForNetCore.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class PermutationTest
    {
        [Fact]
        public void Permutation1Test()
        {
            var input = Enumerable.Range(1, 1);
            var expected = new[] { new int[] { 1 } };

            var actual = PermutationAlgorithms.GetPermutations(input, false);

            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2Test()
        {
            var input = Enumerable.Range(1, 2);
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };

            var actual = PermutationAlgorithms.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2ReversedTest()
        {
            var input = Enumerable.Range(1, 2).Reverse();
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };

            var actual = PermutationAlgorithms.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2WithDuplicationTest()
        {
            var input = new int[] { 1, 1 };
            var expected = new[] { new int[] { 1, 1 } };

            var actual = PermutationAlgorithms.GetPermutations(input, false);
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

            var actual = PermutationAlgorithms.GetPermutations(input, false);
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

            var actual = PermutationAlgorithms.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationEmptyTest()
        {
            var input = Enumerable.Empty<int>();
            var expected = new [] { new int[] { } };

            var actual = PermutationAlgorithms.GetPermutations(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationNullThrowsExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => PermutationAlgorithms.GetPermutations(default(IEnumerable<int>), false).ToArray());            
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
