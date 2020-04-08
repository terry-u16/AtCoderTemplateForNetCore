using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class PermutationTest
    {
        class DummyQuestion : AtCoderQuestionBase
        {
            public override IEnumerable<object> Solve(TextReader inputStream)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ReadOnlyMemory<T>> GetPermutationsForTest<T>(IEnumerable<T> collection, bool isSorted) where T : IComparable<T> => GetPermutations(collection, isSorted);
        }

        [Fact]
        public void Permutation1Test()
        {
            var input = Enumerable.Range(1, 1);
            var expected = new[] { new int[] { 1 } };
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);

            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2Test()
        {
            var input = Enumerable.Range(1, 2);
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2ReversedTest()
        {
            var input = Enumerable.Range(1, 2).Reverse();
            var expected = new[] { new int[] { 1, 2 }, new int[] { 2, 1 } };
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void Permutation2WithDuplicationTest()
        {
            var input = new int[] { 1, 1 };
            var expected = new[] { new int[] { 1, 1 } };
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
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
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
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
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationEmptyTest()
        {
            var input = Enumerable.Empty<int>();
            var expected = new [] { new int[] { } };
            var question = new DummyQuestion();

            var actual = question.GetPermutationsForTest(input, false);
            AssertSequentialEqual(expected, actual);
        }

        [Fact]
        public void PermutationNullThrowsExceptionTest()
        {
            var question = new DummyQuestion();
            Assert.Throws<ArgumentNullException>(() => question.GetPermutationsForTest(default(IEnumerable<int>), false).ToArray());            
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
