using System;
using System.Collections.Generic;
using System.Text;
using AtCoderTemplateForNetCore.Algorithms;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Algorithms
{
    /// <summary>
    /// https://onlinejudge.u-aizu.ac.jp/problems/ALDS1_14_B
    /// </summary>
    public class RollingHashTest
    {
        [Theory]
        [InlineData("aabaaa", "aa", 0, 3, 4)]
        [InlineData("xyzz", "yz", 1)]
        [InlineData("abc", "xyz")]
        [InlineData("iooooioooiooooooioooioooooioooi", "ioooi", 5, 16, 26)]
        [InlineData("babbbbaabbababbaaaaababbaaabbb", "baa", 5, 14, 23)]
        public void StringSearchSliceTest(string t, string p, params int[] indices)
        {
            var tHash = new RollingHash(t);
            var pHash = new RollingHash(p);

            var results = new List<int>();
            var ph = pHash[..];
            for (int begin = 0; begin <= tHash.Length - pHash.Length; begin++)
            {
                if (ph == tHash.Slice(begin, pHash.Length))
                {
                    results.Add(begin);
                }
            }

            Assert.Equal(indices, results);
        }

        [Theory]
        [InlineData("aabaaa", "aa", 0, 3, 4)]
        [InlineData("xyzz", "yz", 1)]
        [InlineData("abc", "xyz")]
        [InlineData("iooooioooiooooooioooioooooioooi", "ioooi", 5, 16, 26)]
        [InlineData("babbbbaabbababbaaaaababbaaabbb", "baa", 5, 14, 23)]
        public void StringSearchRangeTest(string t, string p, params int[] indices)
        {
            var tHash = new RollingHash(t);
            var pHash = new RollingHash(p);

            var results = new List<int>();
            var ph = pHash[..];
            for (int begin = 0; begin <= tHash.Length - pHash.Length; begin++)
            {
                if (ph == tHash[begin..(begin + pHash.Length)])
                {
                    results.Add(begin);
                }
            }

            Assert.Equal(indices, results);
        }

    }
}
