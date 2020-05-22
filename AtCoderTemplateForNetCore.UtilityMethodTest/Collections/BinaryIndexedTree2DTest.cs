using AtCoderTemplateForNetCore.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Collections
{
    public class BinaryIndexedTree2DTest
    {
        [Fact]
        public void BIT2DInitializationZeroTest()
        {
            const int height = 5;
            const int width = 7;
            var bit = new BinaryIndexedTree2D(height, width);
            Assert.Equal(height, bit.Height);
            Assert.Equal(width, bit.Width);
            Assert.Equal(0, bit.Sum(height, width));
        }

        [Fact]
        public void BIT2DSumFromZeroTest()
        {
            const int height = 5;
            const int width = 7;
            var bit = new BinaryIndexedTree2D(height, width);

            var addQueries = new (int row, int column, int value)[] { (3, 2, 8), (1, 5, 4), (0, 6, 3) };
            foreach (var query in addQueries)
            {
                bit.AddAt(query.row, query.column, query.value);
            }

            Assert.Equal(15, bit.Sum(height, width));
            Assert.Equal(4, bit.Sum(2, 6));
            Assert.Equal(0, bit.Sum(0, 0));
            Assert.Equal(0, bit.Sum(1, 1));
        }

        [Fact]
        public void BIT2DSumFromAnyTest()
        {
            const int height = 5;
            const int width = 7;
            var bit = new BinaryIndexedTree2D(height, width);

            var addQueries = new (int row, int column, int value)[] { (3, 2, 8), (1, 5, 4), (0, 6, 3) };
            foreach (var query in addQueries)
            {
                bit.AddAt(query.row, query.column, query.value);
            }

            Assert.Equal(12, bit.Sum(1..4, 2..6));
            Assert.Equal(12, bit.Sum(1, 4, 2, 6));
            Assert.Equal(0, bit.Sum(4..5, 3..4));
            Assert.Equal(0, bit.Sum(4, 5, 3, 4));
        }
    }
}
