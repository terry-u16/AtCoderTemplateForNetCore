using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    public class ZAlgorithmTest
    {
        [Theory]
        [InlineData("momomosumomomosu", 16, 0, 4, 0, 2, 0, 0, 0, 8, 0, 4, 0, 2, 0, 0, 0)]
        [InlineData("momomosumomomomo", 16, 0, 4, 0, 2, 0, 0, 0, 6, 0, 6, 0, 4, 0, 2, 0)]
        [InlineData("momomohimomokusa", 16, 0, 4, 0, 2, 0, 0, 0, 4, 0, 2, 0, 0, 0, 0, 0)]
        public void ZAlgorithmSumomoTest(string input, params int[] expected)
        {
            var z = ZAlgorithm.Search(input);
            Assert.Equal(expected, z);
        }
    }
}
