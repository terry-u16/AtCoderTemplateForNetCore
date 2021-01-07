using System;
using Xunit;
using AtCoderTemplateForNetCore.Problems;
using AtCoderTemplateForNetCore.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class WithIndexTest
    {
        [Fact]
        public void ArrayWithIndexTest()
        {
            var array = new int []{ 5, 1, 0, 3, 9 };
            var i = 0;
            foreach (var (item, index) in array.WithIndex())
            {
                Assert.Equal(array[i], item);
                Assert.Equal(i++, index);
            }
        }
    }
}
