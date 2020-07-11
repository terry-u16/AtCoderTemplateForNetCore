using System;
using AtCoderTemplateForNetCore.Numerics;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class ModMatrixTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 9)]
        [InlineData(100, 200)]
        public void InitializeByHeightWidthTest(int height, int width)
        {
            var matrix = new ModMatrix(height, width);
            Assert.Equal(height, matrix.Height);
            Assert.Equal(width, matrix.Width);
            for (int row = 0; row < height; row++)
                for (int column = 0; column < width; column++)
                    Assert.Equal(0, matrix[row, column]);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public void InitializeByNTest(int n)
        {
            var matrix = new ModMatrix(n);
            Assert.Equal(n, matrix.Height);
            Assert.Equal(n, matrix.Width);
            for (int row = 0; row < n; row++)
                for (int column = 0; column < n; column++)
                    Assert.Equal(0, matrix[row, column]);
        }

        [Fact]
        public void InitializeByJaggedArrayTest()
        {
            var data = new Modular[3][] { 
                new Modular[] { 0, 1 }, 
                new Modular[] { 2, 3 }, 
                new Modular[] { 4, 5 } 
            };

            var matrix = new ModMatrix(data);
            for (int row = 0; row < matrix.Height; row++)
                for (int column = 0; column < matrix.Width; column++)
                    Assert.Equal(data[row][column], matrix[row, column]);
        }

        [Fact]
        public void InitializeByTwoDimensionalArrayTest()
        {
            var data = new Modular[3, 2];
            for (int i = 0; i < 6; i++)
            {
                data[i >> 1, i & 1] = i;
            }

            var matrix = new ModMatrix(data);
            for (int row = 0; row < matrix.Height; row++)
                for (int column = 0; column < matrix.Width; column++)
                    Assert.Equal(data[row, column], matrix[row, column]);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 100)]
        [InlineData(int.MaxValue, int.MinValue)]
        public void InitializeErrorTest(int height, int width) => Assert.Throws<ArgumentOutOfRangeException>(() => new ModMatrix(height, width));
        
        [Fact]
        public void AddTest()
        {
            var data1 = new Modular[3, 2];
            var data2 = new Modular[3, 2];
            for (int i = 0; i < 6; i++)
            {
                data1[i >> 1, i & 1] = i * i;
            }
            for (int i = 0; i < 6; i++)
            {
                data2[i >> 1, i & 1] = i;
            }

            var matrix1 = new ModMatrix(data1);
            var matrix2 = new ModMatrix(data2);
            var added = matrix1 + matrix2;
            for (int row = 0; row < added.Height; row++)
                for (int column = 0; column < added.Width; column++)
                    Assert.Equal(data1[row, column] + data2[row, column], added[row, column]);
        }

        [Fact]
        public void SubtractTest()
        {
            var data1 = new Modular[3, 2];
            var data2 = new Modular[3, 2];
            for (int i = 0; i < 6; i++)
            {
                data1[i >> 1, i & 1] = i * i;
            }
            for (int i = 0; i < 6; i++)
            {
                data2[i >> 1, i & 1] = i;
            }

            var matrix1 = new ModMatrix(data1);
            var matrix2 = new ModMatrix(data2);
            var subtracted = matrix1 - matrix2;
            for (int row = 0; row < subtracted.Height; row++)
                for (int column = 0; column < subtracted.Width; column++)
                    Assert.Equal(data1[row, column] - data2[row, column], subtracted[row, column]);
        }

        [Fact]
        public void MultiplyTest()
        {
            var data1 = new Modular[3, 2];
            var data2 = new Modular[2, 4];
            for (int i = 0; i < 6; i++)
            {
                data1[i >> 1, i & 1] = i;
            }
            for (int i = 0; i < 8; i++)
            {
                data2[i >> 2, i & 11] = i;
            }

            var expected = new Modular[3][]
            {
                new Modular[] { 4, 5, 6, 7 },
                new Modular[] { 12, 17, 22, 27 },
                new Modular[] { 20, 29, 38, 47 }
            };

            var matrix1 = new ModMatrix(data1);
            var matrix2 = new ModMatrix(data2);
            var multiplied = matrix1 * matrix2;
            for (int row = 0; row < multiplied.Height; row++)
                for (int column = 0; column < multiplied.Width; column++)
                    Assert.Equal(expected[row][column], multiplied[row, column]);
        }

        [Fact]
        public void MultiplyVectorTest()
        {
            var matrixData = new Modular[3, 2];
            var vectorData = new Modular[2];
            for (int i = 0; i < 6; i++)
            {
                matrixData[i >> 1, i & 1] = i;
            }
            vectorData[0] = 3;
            vectorData[1] = 7;

            var expected = new Modular[] { 7, 27, 47 };

            var matrix = new ModMatrix(matrixData);
            var vector = new ModVector(vectorData);
            var multiplied = matrix * vector;
            for (int i = 0; i < multiplied.Length; i++)
                Assert.Equal(expected[i], multiplied[i]);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(15, 610)]
        [InlineData(60, 8745084)]
        public void FibonacciTest(int n, long fibonacci)
        {
            var expected = new Modular(fibonacci);
            var initial = new ModVector(2);
            initial[0] = 1;
            var matrix = new ModMatrix(2);
            matrix[0, 0] = 1;
            matrix[0, 1] = 1;
            matrix[1, 0] = 1;
            var pow = matrix.Pow(n);
            var result = (pow * initial)[1];
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(3, 4, 3, 5)]
        [InlineData(3, 4, 5, 4)]
        [InlineData(1, 1, 2, 2)]
        [InlineData(1, 2, 2, 1)]
        public void AddErrorTest(int heightA, int widthA, int heightB, int widthB)
        {
            var a = new ModMatrix(heightA, widthA);
            var b = new ModMatrix(heightB, widthB);
            Assert.Throws<ArgumentException>(() => a + b);
        }

        [Theory]
        [InlineData(3, 4, 3, 5)]
        [InlineData(3, 4, 5, 4)]
        [InlineData(1, 1, 2, 2)]
        [InlineData(1, 2, 2, 1)]
        public void SuctractErrorTest(int heightA, int widthA, int heightB, int widthB)
        {
            var a = new ModMatrix(heightA, widthA);
            var b = new ModMatrix(heightB, widthB);
            Assert.Throws<ArgumentException>(() => a - b);
        }

        [Theory]
        [InlineData(3, 4, 3, 5)]
        [InlineData(3, 4, 5, 4)]
        [InlineData(1, 1, 2, 2)]
        [InlineData(101, 100, 101, 100)]
        public void MultiplyErrorTest(int heightA, int widthA, int heightB, int widthB)
        {
            var a = new ModMatrix(heightA, widthA);
            var b = new ModMatrix(heightB, widthB);
            Assert.Throws<ArgumentException>(() => a * b);
        }

        [Theory]
        [InlineData(3, 4, 3)]
        [InlineData(3, 4, 5)]
        [InlineData(1, 1, 2)]
        [InlineData(101, 100, 101)]
        public void MultiplyVectorErrorTest(int height, int width, int length)
        {
            var a = new ModMatrix(height, width);
            var x = new ModVector(length);
            Assert.Throws<ArgumentException>(() => a * x);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public void IdentityTest(int n)
        {
            var identity = ModMatrix.GetIdentity(n);
            Assert.Equal(n, identity.Height);
            Assert.Equal(n, identity.Width);
            for (int row = 0; row < n; row++)
                for (int column = 0; column < n; column++)
                    Assert.Equal(row == column ? 1 : 0, identity[row, column]);
        }
    }
}
