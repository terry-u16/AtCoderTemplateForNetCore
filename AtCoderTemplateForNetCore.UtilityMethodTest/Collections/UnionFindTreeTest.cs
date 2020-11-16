using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class UnionFindTreeTest
    {
        // See https://atc001.contest.atcoder.jp/tasks/unionfind_a
        [Fact]
        public void UnionFindTree1Test()
        {
            var input =
@"8 9
0 1 2
0 3 2
1 1 3
1 1 4
0 2 4
1 4 1
0 4 2
0 0 0
1 0 0".Split(Environment.NewLine);
            var nq = input[0].Split(' ').Select(int.Parse).ToArray();
            var n = nq[0];
            var q = nq[1];
            var uf = new UnionFind(n);
            var output = new List<string>();

            for (int i = 0; i < q; i++)
            {
                var pab = input[i + 1].Split(' ').Select(int.Parse).ToArray();
                var p = pab[0];
                var a = pab[1];
                var b = pab[2];

                if (p == 0)
                {
                    uf.Unite(a, b);
                }
                else
                {
                    output.Add(uf.IsInSameGroup(a, b) ? "Yes" : "No");
                }
            }

            Assert.Equal(new[] { "Yes", "No", "Yes", "Yes" }, output);

            var sizes = Enumerable.Range(0, n).Select(i => uf.GetGroupSize(i));
            Assert.Equal(new[] { 1, 4, 4, 4, 4, 1, 1, 1 }, sizes);

            Assert.Equal(5, uf.Groups);
        }
    }
}
