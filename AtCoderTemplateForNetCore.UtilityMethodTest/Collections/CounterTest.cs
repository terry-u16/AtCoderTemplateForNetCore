using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtCoderTemplateForNetCore.Collections;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class CounterTest
    {
        [Fact]
        public void CounterIndexerTest()
        {
            var counter = new Counter<int>();
            counter[1] = 100;
            counter[2] = 100;
            counter[2] += 100;
            counter[1]++;
            counter[3]++;
            counter[4] = 10 + counter[4];

            Assert.Equal(101, counter[1]++);
            Assert.Equal(0, counter[5]);
            Assert.Equal(new (int value, long count)[] { (1, 102), (2, 200), (3, 1), (4, 10) }, counter);
        }
    }
}
