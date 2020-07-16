using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtCoderTemplateForNetCore.Collections;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class DequeTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(1000000)]
        public void InitializeWithCapacityTest(int n)
        {
            var deque = new Deque<int>(n);
        }

        [Fact]
        public void InitializeWithCollectionTest()
        {
            const int N = 100000;
            var deque = new Deque<int>(Enumerable.Range(0, N));
            var count = 0;

            Assert.Equal(N, deque.Count);

            foreach (var item in deque)
            {
                Assert.Equal(count++, item);
            }
        }

        [Fact]
        public void EnqueueFirstTest()
        {
            var deque = new Deque<int>();
            foreach (var value in Enumerable.Range(0, 10000))
            {
                deque.EnqueueFirst(value);
            }

            var expected = 9999;
            foreach (var item in deque)
            {
                Assert.Equal(expected--, item);
            }
        }

        [Fact]
        public void EnqueueLastTest()
        {
            var deque = new Deque<int>();
            foreach (var value in Enumerable.Range(0, 10000))
            {
                deque.EnqueueLast(value);
            }

            var expected = 0;
            foreach (var item in deque)
            {
                Assert.Equal(expected++, item);
            }
        }

        [Fact]
        public void DequeueFirstTest()
        {
            var deque = new Deque<int>();
            var stack = new Stack<int>();
            foreach (var value in Enumerable.Range(0, 10000))
            {
                deque.EnqueueFirst(value);
                stack.Push(value);
            }

            foreach (var value in stack)
            {
                Assert.Equal(value, deque.DequeueFirst());
            }
        }

        [Fact]
        public void DequeueLastTest()
        {
            var deque = new Deque<int>();
            var stack = new Stack<int>();
            foreach (var value in Enumerable.Range(0, 10000))
            {
                deque.EnqueueLast(value);
                stack.Push(value);
            }

            foreach (var value in stack)
            {
                Assert.Equal(value, deque.DequeueLast());
            }
        }

        [Fact]
        public void PeekFirstTest()
        {
            var deque = new Deque<int>();
            deque.EnqueueLast(1);
            Assert.Equal(1, deque.PeekFirst());
        }

        [Fact]
        public void PeekLastTest()
        {
            var deque = new Deque<int>();
            deque.EnqueueLast(1);
            Assert.Equal(1, deque.PeekLast());
        }

        [Fact]
        public void DequeueAndPeekThrowsExceptionWhenEmptyTest()
        {
            var deque = new Deque<int>();
            Assert.Throws<InvalidOperationException>(() => deque.DequeueFirst());
            Assert.Throws<InvalidOperationException>(() => deque.DequeueLast());
            Assert.Throws<InvalidOperationException>(() => deque.PeekFirst());
            Assert.Throws<InvalidOperationException>(() => deque.PeekLast());
        }

        [Fact]
        public void IndexerTest()
        {
            const int N = 7;
            var deque = new Deque<int>();
            foreach (var value in Enumerable.Range(0, N))
            {
                deque.EnqueueLast(value);
            }

            for (int i = 0; i < N; i++)
            {
                Assert.Equal(i, deque[i]);
            }

            for (int i = 0; i < N; i++)
            {
                Assert.Equal(N - 1 - i, deque[^(i + 1)]);
            }
        }

        [Fact]
        public void IndexerThrowsExceptionTest()
        {
            var deque = new Deque<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => deque[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => deque[^1]);
            deque.EnqueueFirst(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => deque[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => deque[^0]);
        }

        [Fact]
        public void DequeueWhenCountIsZeroThrowsExceptionTest()
        {
            var deque = new Deque<int>();
            Assert.Throws<InvalidOperationException>(() => deque.DequeueFirst());
            Assert.Throws<InvalidOperationException>(() => deque.DequeueLast());
        }
    }
}
