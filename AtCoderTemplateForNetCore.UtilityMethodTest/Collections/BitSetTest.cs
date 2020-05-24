using AtCoderTemplateForNetCore.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class BitSetTest
    {
        [Theory]
        [InlineData(0x0f0f0f0f, 0xf0f0f0f0)]
        [InlineData(0x1, 0x80000000)]
        public void ReverseTest(uint input, uint output)
        {
            Assert.Equal(output, new BitSet(input).Reverse());
        }

        [Theory]
        [InlineData(0x0f0f0f0f, 16)]
        [InlineData(0x0, 0)]
        public void PopCountTest(uint input, int count)
        {
            Assert.Equal(count, new BitSet(input).Count());
        }

        [Theory]
        [InlineData(0xf0, 0x10)]
        public void LsbTest(uint input, uint lsb)
        {
            Assert.Equal(lsb, new BitSet(input).Lsb());
        }

        [Fact]
        public void IndexerTest()
        {
            var bitSet = new BitSet(0b0101);
            Assert.True(bitSet[0]);
            Assert.False(bitSet[1]);
            Assert.True(bitSet[2]);
            Assert.False(bitSet[3]);
        }

        [Theory]
        [InlineData(0b0001, 2, true, 0b0101)]
        [InlineData(0b0001, 2, false, 0b0001)]
        [InlineData(0b0001, 0, true, 0b0001)]
        [InlineData(0b0001, 0, false, 0b0000)]
        public void SetAtTest(uint input, int digit, bool value, uint expected)
        {
            Assert.Equal(expected, new BitSet(input).SetAt(digit, value));
        }
    }
}
