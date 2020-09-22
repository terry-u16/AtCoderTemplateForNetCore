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
        public void ArrayFill1Test()
        {
            const int N = 10;
            var array = new int[N].Fill(10);
            Assert.Equal(Enumerable.Repeat(10, N), array);
        }

        [Fact]
        public void ArrayFill2Test()
        {
            const int N = 10;
            var array = new int[N, N].Fill(10);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Assert.Equal(10, array[i, j]);
                }
            }
        }
    }
}
