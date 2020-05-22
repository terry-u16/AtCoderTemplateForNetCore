using AtCoderTemplateForNetCore.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class BinaryIndexedTreeTest
    {
        [Fact]
        public void BITInitializationZeroTest()
        {
            const int length = 5;
            var bit = new BinaryIndexedTree(length);
            Assert.Equal(length, bit.Length);
            Assert.Equal(0, bit.Sum(length));
        }

        [Fact]
        public void BITSumFromZeroTest()
        {
            var a = new long[] { 5, 3, 7, 9, 8 };
            long[] sum = GetPrefixSum(a);

            var bit = new BinaryIndexedTree(a);

            Assert.Equal(a.Length, bit.Length);
            for (int i = 0; i < sum.Length; i++)
            {
                Assert.Equal(sum[i], bit.Sum(i));
            }
        }

        [Fact]
        public void BITAddTest()
        {
            var a = new long[] { 5, 3, 7, 9, 8 };
            var sum = GetPrefixSum(a);

            var bit = new BinaryIndexedTree(a);

            Assert.Equal(a.Length, bit.Length);
            for (int i = 0; i < sum.Length; i++)
            {
                Assert.Equal(sum[i], bit.Sum(i));
            }

            bit.AddAt(2, 3);
            a[2] += 3;
            sum = GetPrefixSum(a);

            Assert.Equal(a.Length, bit.Length);
            for (int i = 0; i < sum.Length; i++)
            {
                Assert.Equal(sum[i], bit.Sum(i));
                Assert.Equal(sum[i], bit.Sum(^(a.Length - i)));
            }
        }

        [Fact]
        public void BITSumFromAnyTest()
        {
            var a = new long[] { 5, 3, 7, 9, 8 };
            var sum = GetPrefixSum(a);

            var bit = new BinaryIndexedTree(a);

            Assert.Equal(a.Length, bit.Length);
            for (int begin = 0; begin < bit.Length; begin++)
            {
                for (int end = begin; end <= bit.Length; end++)
                {
                    Assert.Equal(sum[end] - sum[begin], bit.Sum(begin, end));
                }
            }
        }

        [Fact]
        public void BITLowerBoundTest()
        {
            var a = new long[] { 5, 3, 7, 9, 8 };
            var sum = GetPrefixSum(a);

            var bit = new BinaryIndexedTree(a);

            Assert.Equal(0, bit.GetLowerBound(0));
            Assert.Equal(0, bit.GetLowerBound(3));
            Assert.Equal(1, bit.GetLowerBound(8));
            Assert.Equal(2, bit.GetLowerBound(9));
            Assert.Equal(3, bit.GetLowerBound(18));
            Assert.Equal(5, bit.GetLowerBound(100));
        }

        [Fact]
        public void OutOfRangeThrowsExceptionTest()
        {
            const int length = 5;
            var bit = new BinaryIndexedTree(length);

            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(^6, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(5, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(^6));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(6));
        }

        private static long[] GetPrefixSum(long[] a)
        {
            var sum = new long[a.Length + 1];
            for (int i = 0; i < a.Length; i++)
            {
                sum[i + 1] = sum[i] + a[i];
            }

            return sum;
        }
    }
}
