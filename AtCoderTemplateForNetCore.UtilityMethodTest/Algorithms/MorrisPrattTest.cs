using System;
using System.Collections.Generic;
using System.Text;
using AtCoderTemplateForNetCore.Algorithms;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    public class MorrisPrattTest
    {
        [Theory]
        [InlineData("abcab", "ababcabcabcc", 2, 5)]
        [InlineData("abcab", "abcabcabcabcc", 0, 3, 6)]
        [InlineData("abcab", "abcab", 0)]
        [InlineData("aaa", "aaaaaa", 0, 1, 2, 3)]
        [InlineData("aaaaaa", "aaaaaa", 0)]
        [InlineData("aaaaaaa", "aaaaaa")]
        public void MorrisPrattStringTest(string search, string target, params int[] expected)
        {
            var mp = new MorrisPratt<char>(search);
            var result = mp.SearchAll(target);
            Assert.Equal(expected, result);
        }
    }
}
