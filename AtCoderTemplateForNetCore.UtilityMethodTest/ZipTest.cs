using System;
using Xunit;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest
{
    public class ZipTest
    {
        [Fact]
        public void ZipTwoSequencesTest()
        {
            var expected = new[] { (1, 2), (2, 3), (3, 4), (4, 5), (5, 6) };
            var oneToSix = Enumerable.Range(1, 6);

            Assert.Equal(expected, (oneToSix.Take(5), oneToSix.Skip(1)).Zip());
        }

        [Fact]
        public void ZipThreeSequencesTest()
        {
            var expected = new[] { (1, 2, 3), (2, 3, 4), (3, 4, 5), (4, 5, 6), (5, 6, 7) };
            var oneToSix = Enumerable.Range(1, 7);

            Assert.Equal(expected, (oneToSix, oneToSix.Skip(1), oneToSix.Skip(2)).Zip());
        }
    }
}
