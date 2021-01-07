using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
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
        public void ArrayFill2Test()
        {
            const int N = 10;
            var array = new int[N, N];
            array.Fill(10);

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
