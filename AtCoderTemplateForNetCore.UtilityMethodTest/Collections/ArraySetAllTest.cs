using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class ArraySetAllTest
    {
        [Fact]
        public void ArraySetAll1Test()
        {
            const int N = 10;
            var array = new int[N].SetAll(i => i);
            Assert.Equal(Enumerable.Range(0, 10), array);
        }

        [Fact]
        public void ArraySetAll2Test()
        {
            const int N = 10;
            var array = new int[N, N].SetAll((i, j) => i + j);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Assert.Equal(i + j, array[i, j]);
                }
            }
        }
    }
}
