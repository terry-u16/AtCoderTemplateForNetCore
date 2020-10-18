using Xunit;
using ModInt = AtCoderTemplateForNetCore.Numerics.StaticModInt<AtCoderTemplateForNetCore.Numerics.Mod1000000007>;

namespace AtCoderTemplateForNetCore.UtilityMethodTest.Numerics
{
    public class ModIntTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1000000010, 3)]
        [InlineData(-3, 1000000004)]
        [InlineData(2147483662032385532, 3)]
        public void InitializationTest(long value, int expected)
        {
            var m = new ModInt(value);
            Assert.Equal(expected, m.Value);
            Assert.Equal($"{m.Value}", m.ToString());
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(1000000000, 10, 3)]
        [InlineData(1000000006, 1, 0)]
        public void AddTest(long a, long b, int expected)
        {
            var result = new ModInt(a) + new ModInt(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(2, 3, 1000000006)]
        public void SubtractTest(long a, long b, int expected)
        {
            var result = new ModInt(a) - new ModInt(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(10000, 100001, 9993)]
        public void MultiplicationTest(long a, long b, int expected)
        {
            var result = new ModInt(a) * new ModInt(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(6, 3, 2)]
        [InlineData(100, 10, 10)]
        [InlineData(1000000010, 100000001, 10)]
        public void DivisionTest(long a, long b, int expected)
        {
            var result = new ModInt(a) / new ModInt(b);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(3, 3, true)]
        [InlineData(2, 3, false)]
        [InlineData(1000000010, 3, true)]
        public void EqualsTest(long a, long b, bool expected)
        {
            var modA = new ModInt(a);
            var modB = new ModInt(b);
            Assert.Equal(expected, modA == modB);
            Assert.Equal(!expected, modA != modB);
            Assert.Equal(expected, modA.Equals(modB));
            Assert.Equal(expected, modA.Equals(modB as object));
        }

        [Theory]
        [InlineData(2, 3, 8)]
        [InlineData(10, 10, 999999937)]
        public void PowTest(int a, int n, int expected)
        {
            var pow = new ModInt(a).Pow(n);
            Assert.Equal(expected, pow.Value);
        }
    }
}
