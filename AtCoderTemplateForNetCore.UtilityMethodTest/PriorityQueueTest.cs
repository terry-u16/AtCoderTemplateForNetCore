using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class PriorityQueueTest
    {
        [Fact]
        public void DescendingEnqueueDequeueTest()
        {
            var values = new[] { 4, 1, 3, 0, 2, 5, 8, 11, 10 };
            var queue = new PriorityQueue<int>();

            Assert.True(queue.IsDescending);

            int count = 0;
            foreach (var value in values)
            {
                queue.Enqueue(value);
                Assert.Equal(++count, queue.Count);
            }

            Assert.Equal(values.Length, queue.Count);

            Array.Sort(values);
            Array.Reverse(values);

            for (int i = 0; i < values.Length; i++)
            {
                Assert.Equal(values[i], queue.Dequeue());
                Assert.Equal(--count, queue.Count);
            }
        }

        [Theory]
        [InlineData(42, 10)]
        [InlineData(255, 1000)]
        [InlineData(2020, 1000000)]
        public void DescendingEnqueueDequeueRandomTest(int seed, int length)
        {
            var priorityQueue = new PriorityQueue<int>();
            EnqueDequeRandomTest(seed, length, priorityQueue, true);
        }

        [Theory]
        [InlineData(42, 10)]
        [InlineData(255, 1000)]
        [InlineData(2020, 100000)]
        public void AscendingEnqueueDequeueRandomTest(int seed, int length)
        {
            var priorityQueue = new PriorityQueue<int>(false);
            EnqueDequeRandomTest(seed, length, priorityQueue, false);
        }


        private void EnqueDequeRandomTest(int seed, int length, PriorityQueue<int> priorityQueue, bool descending)
        {
            var random = new Random(seed);
            var values = new int[length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = random.Next();
            }
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            foreach (var value in values)
            {
                priorityQueue.Enqueue(value);
            }

            Array.Sort(values);
            if (descending)
            {
                Array.Reverse(values);
            }

            Assert.Equal(values, priorityQueue);
            Assert.Equal(length, priorityQueue.Count);
        }

        [Fact]
        public void EnumerableCancellTest()
        {
            var values = new[] { 4, 1, 3, 0, 2, 5, 8, 11, 10 };
            var queue = new PriorityQueue<int>(values);

            Array.Sort(values);
            Array.Reverse(values);

            foreach (var value in queue.Take(5))
            {
            }

            Assert.Equal(values.Length, queue.Count);
        }
    }
}
