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

        [Fact]
        public void BIT2DIndexerTest()
        {
            const int height = 5;
            const int width = 7;
            var bit = new BinaryIndexedTree2D(height, width);

            var addQueries = new (int row, int column, int value)[] { (3, 2, 8), (1, 5, 4), (0, 6, 3) };
            foreach (var query in addQueries)
            {
                bit.AddAt(query.row, query.column, query.value);
            }

            Assert.Equal(8, bit[3, 2]);
            Assert.Equal(4, bit[1, 5]);
            Assert.Equal(3, bit[0, 6]);
            Assert.Equal(0, bit[1, 1]);

            bit[1, 1] = 5;
            bit[3, 2] = 10;
            bit[1, 5] -= 2;
            Assert.Equal(20, bit.Sum(^0, ^0));
        }

        [Fact]
        public void PlanetaryExplorationTest()
        {
            // JOI 2010 本戦
            const int height = 4;
            const int width = 7;
            var s = new string[] { "JIOJOIJ", "IOJOIJO", "JOIJOOI", "OOJJIJO" };

            var jBit = new BinaryIndexedTree2D(height, width);
            var oBit = new BinaryIndexedTree2D(height, width);
            var iBit = new BinaryIndexedTree2D(height, width);

            for (int row = 0; row < s.Length; row++)
            {
                for (int column = 0; column < s[row].Length; column++)
                {
                    var bit = s[row][column] switch
                    {
                        'J' => jBit,
                        'O' => oBit,
                        'I' => iBit,
                        _ => null
                    };
                    bit.AddAt(row, column, 1);
                }
            }

            var rowRange = 1..3;
            var columnRange = 1..6;
            Assert.Equal(3, jBit.Sum(rowRange, columnRange));
            Assert.Equal(5, oBit.Sum(rowRange, columnRange));
            Assert.Equal(2, iBit.Sum(rowRange, columnRange));

            rowRange = ^3..^1;
            columnRange = ^6..^1;
            Assert.Equal(3, jBit.Sum(rowRange, columnRange));
            Assert.Equal(5, oBit.Sum(rowRange, columnRange));
            Assert.Equal(2, iBit.Sum(rowRange, columnRange));
        }

        [Fact]
        public void OutOfRangeThrowsException()
        {
            const int height = 5;
            const int width = 7;
            var bit = new BinaryIndexedTree2D(height, width);

            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(-1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(5, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.AddAt(0, 7, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(7, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(^6, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(1, 9));
            Assert.Throws<ArgumentOutOfRangeException>(() => bit.Sum(1, ^8));
        }
    }
}
