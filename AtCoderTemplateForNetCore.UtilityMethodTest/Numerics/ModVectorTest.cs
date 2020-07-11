using System;
using System.Collections.Generic;
using System.Text;
using AtCoderTemplateForNetCore.Numerics;
using Xunit;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class ModVectorTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(100)]
        public void InitializeByLengthTest(int length)
        {
            var vector = new ModVector(length);
            Assert.Equal(length, vector.Length);
        }

        [Fact]
        public void InitializeByArrayTest()
        {
            var data = new Modular[] { 1, 2, 3, 4, 5 };
            var vector = new ModVector(data);
            for (int i = 0; i < vector.Length; i++)
                Assert.Equal(data[i], vector[i]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void InitializeErrorTest(int length) => Assert.Throws<ArgumentOutOfRangeException>(() => new ModVector(length));

        [Fact]
        public void AddTest()
        {
            var data1 = new Modular[5];
            var data2 = new Modular[5];
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = i * i;
                data2[i] = i;
            }
            var vector1 = new ModVector(data1);
            var vector2 = new ModVector(data2);

            var result = vector1 + vector2;
            for (int i = 0; i < result.Length; i++)
                Assert.Equal(data1[i] + data2[i], result[i]);
        }

        [Fact]
        public void SubtractTest()
        {
            var data1 = new Modular[5];
            var data2 = new Modular[5];
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = i * i;
                data2[i] = i;
            }
            var vector1 = new ModVector(data1);
            var vector2 = new ModVector(data2);

            var result = vector1 - vector2;
            for (int i = 0; i < result.Length; i++)
                Assert.Equal(data1[i] - data2[i], result[i]);
        }

        [Fact]
        public void MultiplyTest()
        {
            var data1 = new Modular[5];
            var data2 = new Modular[5];
            var expected = Modular.Zero;
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = i * i;
                data2[i] = i;
                expected += data1[i] * data2[i];
            }
            var vector1 = new ModVector(data1);
            var vector2 = new ModVector(data2);

            var result = vector1 * vector2;
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 2)]
        public void AddErrorTest(int length1, int length2)
        {
            var a = new ModVector(length1);
            var b = new ModVector(length2);
            Assert.Throws<ArgumentException>(() => a + b);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 2)]
        public void SubtractErrorTest(int length1, int length2)
        {
            var a = new ModVector(length1);
            var b = new ModVector(length2);
            Assert.Throws<ArgumentException>(() => a - b);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 2)]
        public void MultiplyErrorTest(int length1, int length2)
        {
            var a = new ModVector(length1);
            var b = new ModVector(length2);
            Assert.Throws<ArgumentException>(() => a * b);
        }

    }
}
