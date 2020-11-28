using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using AtCoderTemplateForNetCore.Numerics;
using AtCoderTemplateForNetCore.Questions;

namespace AtCoderTemplateForNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IAtCoderQuestion question = new QuestionA();
            using var io = new IOManager(Console.OpenStandardInput(), Console.OpenStandardOutput());
            question.Solve(io);
        }
    }
}

#region Base Class

namespace AtCoderTemplateForNetCore.Questions
{
    public interface IAtCoderQuestion
    {
        string Solve(string input);
        void Solve(IOManager io);
    }

    public abstract class AtCoderQuestionBase : IAtCoderQuestion
    {
        public string Solve(string input)
        {
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var outputStream = new MemoryStream();
            using var manager = new IOManager(inputStream, outputStream);

            Solve(manager);
            manager.Flush();

            outputStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(outputStream);
            return reader.ReadToEnd();
        }

        public abstract void Solve(IOManager io);
    }
}

#endregion

#region Utils

namespace AtCoderTemplateForNetCore
{
    public class IOManager : IDisposable
    {
        private readonly BinaryReader _reader;
        private readonly StreamWriter _writer;
        private bool _disposedValue;
        private byte[] _buffer = new byte[1024];
        private int _length;
        private int _cursor;
        private bool _eof;

        const char ValidFirstChar = '!';
        const char ValidLastChar = '~';

        public IOManager(Stream input, Stream output)
        {
            _reader = new BinaryReader(input);
            _writer = new StreamWriter(output) { AutoFlush = false };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char ReadAscii()
        {
            if (_cursor == _length)
            {
                _cursor = 0;
                _length = _reader.Read(_buffer);

                if (_length == 0)
                {
                    if (!_eof)
                    {
                        _eof = true;
                        return char.MinValue;
                    }
                    else
                    {
                        ThrowEndOfStreamException();
                    }
                }
            }

            return (char)_buffer[_cursor++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            char c;
            while (!IsValidChar(c = ReadAscii())) { }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            var builder = new StringBuilder();
            char c;
            while (!IsValidChar(c = ReadAscii())) { }

            do
            {
                builder.Append(c);
            } while (IsValidChar(c = ReadAscii()));

            return builder.ToString();
        }

        public int ReadInt() => (int)ReadLong();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            long result = 0;
            bool isPositive = true;
            char c;

            while (!IsNumericChar(c = ReadAscii())) { }

            if (c == '-')
            {
                isPositive = false;
                c = ReadAscii();
            }

            do
            {
                result *= 10;
                result += c - '0';
            } while (IsNumericChar(c = ReadAscii()));

            return isPositive ? result : -result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<char> ReadChunk(Span<char> span)
        {
            var i = 0;
            char c;
            while (!IsValidChar(c = ReadAscii())) { }

            do
            {
                span[i++] = c;
            } while (IsValidChar(c = ReadAscii()));

            return span.Slice(0, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() => double.Parse(ReadChunk(stackalloc char[32]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ReadDecimal() => decimal.Parse(ReadChunk(stackalloc char[32]));

        public int[] ReadIntArray(int n)
        {
            var a = new int[n];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ReadInt();
            }
            return a;
        }

        public long[] ReadLongArray(int n)
        {
            var a = new long[n];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ReadLong();
            }
            return a;
        }

        public double[] ReadDoubleArray(int n)
        {
            var a = new double[n];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ReadDouble();
            }
            return a;
        }

        public decimal[] ReadDecimalArray(int n)
        {
            var a = new decimal[n];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ReadDecimal();
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value) => _writer.Write(value.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine<T>(T value) => _writer.WriteLine(value.ToString());

        public void WriteLine<T>(IEnumerable<T> values, char separator)
        {
            var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                _writer.Write(e.Current.ToString());

                while (e.MoveNext())
                {
                    _writer.Write(separator);
                    _writer.Write(e.Current.ToString());
                }
            }

            _writer.WriteLine();
        }

        public void WriteLine<T>(T[] values, char separator) => WriteLine((ReadOnlySpan<T>)values, separator);
        public void WriteLine<T>(Span<T> values, char separator) => WriteLine((ReadOnlySpan<T>)values, separator);

        public void WriteLine<T>(ReadOnlySpan<T> values, char separator)
        {
            for (int i = 0; i < values.Length - 1; i++)
            {
                _writer.Write(values[i]);
                _writer.Write(separator);
            }

            if (values.Length > 0)
            {
                _writer.Write(values[^1]);
            }

            _writer.WriteLine();
        }

        public void Flush() => _writer.Flush();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidChar(char c) => ValidFirstChar <= c && c <= ValidLastChar;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNumericChar(char c) => ('0' <= c && c <= '9') || c == '-';

        private void ThrowEndOfStreamException() => throw new EndOfStreamException();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reader.Dispose();
                    _writer.Flush();
                    _writer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public static class UtilExtensions
    {
        public static bool ChangeMax<T>(ref this T value, T other) where T : struct, IComparable<T>
        {
            if (value.CompareTo(other) < 0)
            {
                value = other;
                return true;
            }
            return false;
        }

        public static bool ChangeMin<T>(ref this T value, T other) where T : struct, IComparable<T>
        {
            if (value.CompareTo(other) > 0)
            {
                value = other;
                return true;
            }
            return false;
        }

        public static void SwapIfLargerThan<T>(ref this T a, ref T b) where T : struct, IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                (a, b) = (b, a);
            }
        }

        public static void SwapIfSmallerThan<T>(ref this T a, ref T b) where T : struct, IComparable<T>
        {
            if (a.CompareTo(b) < 0)
            {
                (a, b) = (b, a);
            }
        }

        public static void Sort<T>(this T[] array) where T : IComparable<T> => Array.Sort(array);
        public static void Sort<T>(this T[] array, Comparison<T> comparison) => Array.Sort(array, comparison);
    }

    public static class CollectionExtensions
    {
        private class ArrayWrapper<T>
        {
#pragma warning disable CS0649
            public T[] Array;
#pragma warning restore CS0649
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return Unsafe.As<ArrayWrapper<T>>(list).Array.AsSpan(0, list.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this T[,] array, int i)
        {
            var width = array.GetLength(1);
            return MemoryMarshal.CreateSpan(ref array[i, 0], width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this T[,,] array, int i, int j)
        {
            var width = array.GetLength(2);
            return MemoryMarshal.CreateSpan(ref array[i, j, 0], width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this T[,,,] array, int i, int j, int k)
        {
            var width = array.GetLength(3);
            return MemoryMarshal.CreateSpan(ref array[i, j, k, 0], width);
        }

        public static void Fill<T>(this T[] array, T value) => array.AsSpan().Fill(value);
        public static void Fill<T>(this T[,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0], array.Length).Fill(value);
        public static void Fill<T>(this T[,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length).Fill(value);
        public static void Fill<T>(this T[,,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length).Fill(value);
    }

    public static class SearchExtensions
    {
        struct LowerBoundComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y) => 0 <= x.CompareTo(y) ? 1 : -1;
        }

        struct UpperBoundComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y) => 0 < x.CompareTo(y) ? 1 : -1;
        }

        // https://trsing.hatenablog.com/entry/2019/08/27/211038
        public static int GetGreaterEqualIndex<T>(this ReadOnlySpan<T> span, T inclusiveMin) where T : IComparable<T> => ~span.BinarySearch(inclusiveMin, new UpperBoundComparer<T>());
        public static int GetGreaterThanIndex<T>(this ReadOnlySpan<T> span, T exclusiveMin) where T : IComparable<T> => ~span.BinarySearch(exclusiveMin, new LowerBoundComparer<T>());
        public static int GetLessEqualIndex<T>(this ReadOnlySpan<T> span, T inclusiveMax) where T : IComparable<T> => ~span.BinarySearch(inclusiveMax, new LowerBoundComparer<T>()) - 1;
        public static int GetLessThanIndex<T>(this ReadOnlySpan<T> span, T exclusiveMax) where T : IComparable<T> => ~span.BinarySearch(exclusiveMax, new UpperBoundComparer<T>()) - 1;
        public static int GetGreaterEqualIndex<T>(this Span<T> span, T inclusiveMin) where T : IComparable<T> => ((ReadOnlySpan<T>)span).GetGreaterEqualIndex(inclusiveMin);
        public static int GetGreaterThanIndex<T>(this Span<T> span, T exclusiveMin) where T : IComparable<T> => ((ReadOnlySpan<T>)span).GetGreaterThanIndex(exclusiveMin);
        public static int GetLessEqualIndex<T>(this Span<T> span, T inclusiveMax) where T : IComparable<T> => ((ReadOnlySpan<T>)span).GetLessEqualIndex(inclusiveMax);
        public static int GetLessThanIndex<T>(this Span<T> span, T exclusiveMax) where T : IComparable<T> => ((ReadOnlySpan<T>)span).GetLessThanIndex(exclusiveMax);

        public static int BoundaryBinarySearch(Predicate<int> predicate, int ok, int ng)
        {
            while (Math.Abs(ok - ng) > 1)
            {
                int mid = (ok + ng) / 2;
                if (predicate(mid))
                {
                    ok = mid;
                }
                else
                {
                    ng = mid;
                }
            }
            return ok;
        }

        public static long BoundaryBinarySearch(Predicate<long> predicate, long ok, long ng)
        {
            while (Math.Abs(ok - ng) > 1)
            {
                long mid = (ok + ng) / 2;
                if (predicate(mid))
                {
                    ok = mid;
                }
                else
                {
                    ng = mid;
                }
            }
            return ok;
        }

        public static double Bisection(Func<double, double> f, double a, double b, double eps = 1e-9)
        {
            if (f(a) * f(b) >= 0)
            {
                throw new ArgumentException("f(a)とf(b)は異符号である必要があります。");
            }

            const int maxLoop = 100;
            double mid = (a + b) / 2;

            for (int i = 0; i < maxLoop; i++)
            {
                if (f(a) * f(mid) < 0)
                {
                    b = mid;
                }
                else
                {
                    a = mid;
                }
                mid = (a + b) / 2;
                if (Math.Abs(b - a) < eps)
                {
                    break;
                }
            }
            return mid;
        }
    }
}

#endregion

#region Algorithm

namespace AtCoderTemplateForNetCore.Numerics
{
    public static class NumericalAlgorithms
    {
        public static long Gcd(long a, long b)
        {
            if (a < b)
            {
                (a, b) = (b, a);
            }

            if (b > 0)
            {
                return Gcd(b, a % b);
            }
            else if (b == 0)
            {
                return a;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"{nameof(a)}, {nameof(b)}は0以上の整数である必要があります。");
            }
        }

        public static long Lcm(long a, long b)
        {
            if (a < 0 || b < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(a)}, {nameof(b)}は0以上の整数である必要があります。");
            }

            return a / Gcd(a, b) * b;
        }

        public static IEnumerable<long> GetDivisiors(long n)
        {
            var lastHalf = new Stack<long>();
            for (long i = 1; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    yield return i;
                    if (i * i != n)
                    {
                        lastHalf.Push(n / i);
                    }
                }
            }

            while (lastHalf.Count > 0)
            {
                yield return lastHalf.Pop();
            }
        }

        public static IEnumerable<(long prime, int count)> PrimeFactorize(long n)
        {
            for (long p = 2; p * p <= n; p++)
            {
                var count = 0;

                while (n % p == 0)
                {
                    count++;
                    n /= p;
                }

                if (count > 0)
                {
                    yield return (p, count);
                }
            }

            if (n > 1)
            {
                yield return (n, 1);
            }
        }
    }

    #region ModInt

    /// <summary>
    /// コンパイル時に決定する mod を表します。
    /// </summary>
    /// <example>
    /// <code>
    /// public readonly struct Mod1000000009 : IStaticMod
    /// {
    ///     public uint Mod => 1000000009;
    ///     public bool IsPrime => true;
    /// }
    /// </code>
    /// </example>
    public interface IStaticMod
    {
        /// <summary>
        /// mod を取得します。
        /// </summary>
        uint Mod { get; }

        /// <summary>
        /// mod が素数であるか識別します。
        /// </summary>
        bool IsPrime { get; }
    }

    public readonly struct Mod1000000007 : IStaticMod
    {
        public uint Mod => 1000000007;
        public bool IsPrime => true;
    }

    public readonly struct Mod998244353 : IStaticMod
    {
        public uint Mod => 998244353;
        public bool IsPrime => true;
    }

    /// <summary>
    /// 実行時に決定する mod の ID を表します。
    /// </summary>
    /// <example>
    /// <code>
    /// public readonly struct ModID123 : IDynamicModID { }
    /// </code>
    /// </example>
    public interface IDynamicModID { }

    public readonly struct ModID0 : IDynamicModID { }
    public readonly struct ModID1 : IDynamicModID { }
    public readonly struct ModID2 : IDynamicModID { }

    /// <summary>
    /// 四則演算時に自動で mod を取る整数型。mod の値はコンパイル時に決定している必要があります。
    /// </summary>
    /// <typeparam name="T">定数 mod を表す構造体</typeparam>
    /// <example>
    /// <code>
    /// using ModInt = AtCoder.StaticModInt&lt;AtCoder.Mod1000000007&gt;;
    ///
    /// void SomeMethod()
    /// {
    ///     var m = new ModInt(1);
    ///     m -= 2;
    ///     Console.WriteLine(m);   // 1000000006
    /// }
    /// </code>
    /// </example>
    public readonly struct StaticModInt<T> : IEquatable<StaticModInt<T>> where T : struct, IStaticMod
    {
        private readonly uint _v;

        /// <summary>
        /// 格納されている値を返します。
        /// </summary>
        public int Value => (int)_v;

        /// <summary>
        /// mod を返します。
        /// </summary>
        public static int Mod => (int)default(T).Mod;

        public static StaticModInt<T> Zero => new StaticModInt<T>();
        public static StaticModInt<T> One => new StaticModInt<T>(1u);

        /// <summary>
        /// <paramref name="v"/> に対して mod を取らずに StaticModInt&lt;<typeparamref name="T"/>&gt; 型のインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// <para>定数倍高速化のための関数です。 <paramref name="v"/> に 0 未満または mod 以上の値を入れた場合の挙動は未定義です。</para>
        /// <para>制約: 0≤|<paramref name="v"/>|&lt;mod</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> Raw(int v)
        {
            var u = unchecked((uint)v);
            Debug.Assert(u < Mod);
            return new StaticModInt<T>(u);
        }

        /// <summary>
        /// StaticModInt&lt;<typeparamref name="T"/>&gt; 型のインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="v"/>が 0 未満、もしくは mod 以上の場合、自動で mod を取ります。
        /// </remarks>
        public StaticModInt(long v) : this(Round(v)) { }

        private StaticModInt(uint v) => _v = v;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Round(long v)
        {
            var x = v % default(T).Mod;
            if (x < 0)
            {
                x += default(T).Mod;
            }
            return (uint)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> operator ++(StaticModInt<T> value)
        {
            var v = value._v + 1;
            if (v == default(T).Mod)
            {
                v = 0;
            }
            return new StaticModInt<T>(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> operator --(StaticModInt<T> value)
        {
            var v = value._v;
            if (v == 0)
            {
                v = default(T).Mod;
            }
            return new StaticModInt<T>(v - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> operator +(StaticModInt<T> lhs, StaticModInt<T> rhs)
        {
            var v = lhs._v + rhs._v;
            if (v >= default(T).Mod)
            {
                v -= default(T).Mod;
            }
            return new StaticModInt<T>(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> operator -(StaticModInt<T> lhs, StaticModInt<T> rhs)
        {
            unchecked
            {
                var v = lhs._v - rhs._v;
                if (v >= default(T).Mod)
                {
                    v += default(T).Mod;
                }
                return new StaticModInt<T>(v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticModInt<T> operator *(StaticModInt<T> lhs, StaticModInt<T> rhs)
        {
            return new StaticModInt<T>((uint)((ulong)lhs._v * rhs._v % default(T).Mod));
        }

        /// <summary>
        /// 除算を行います。
        /// </summary>
        /// <remarks>
        /// <para>- 制約: <paramref name="rhs"/> に乗法の逆元が存在する。（gcd(<paramref name="rhs"/>, mod) = 1）</para>
        /// <para>- 計算量: O(log(mod))</para>
        /// </remarks>
        public static StaticModInt<T> operator /(StaticModInt<T> lhs, StaticModInt<T> rhs) => lhs * rhs.Inverse();

        public static StaticModInt<T> operator +(StaticModInt<T> value) => value;
        public static StaticModInt<T> operator -(StaticModInt<T> value) => new StaticModInt<T>() - value;
        public static bool operator ==(StaticModInt<T> lhs, StaticModInt<T> rhs) => lhs._v == rhs._v;
        public static bool operator !=(StaticModInt<T> lhs, StaticModInt<T> rhs) => lhs._v != rhs._v;
        public static implicit operator StaticModInt<T>(int value) => new StaticModInt<T>(value);
        public static implicit operator StaticModInt<T>(long value) => new StaticModInt<T>(value);

        /// <summary>
        /// 自身を x として、x^<paramref name="n"/> を返します。
        /// </summary>
        /// <remarks>
        /// <para>制約: 0≤|<paramref name="n"/>|</para>
        /// <para>計算量: O(log(<paramref name="n"/>))</para>
        /// </remarks>
        public StaticModInt<T> Pow(long n)
        {
            Debug.Assert(0 <= n);
            var x = this;
            var r = Raw(1);

            while (n > 0)
            {
                if ((n & 1) > 0)
                {
                    r *= x;
                }
                x *= x;
                n >>= 1;
            }

            return r;
        }

        /// <summary>
        /// 自身を x として、 xy≡1 なる y を返します。
        /// </summary>
        /// <remarks>
        /// <para>制約: gcd(x, mod) = 1</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StaticModInt<T> Inverse()
        {
            if (default(T).IsPrime)
            {
                Debug.Assert(_v > 0);
                return Pow(default(T).Mod - 2);
            }
            else
            {
                var (g, x) = InternalMath.InvGCD(_v, default(T).Mod);
                Debug.Assert(g == 1);
                return new StaticModInt<T>(x);
            }
        }

        public override string ToString() => _v.ToString();
        public override bool Equals(object obj) => obj is StaticModInt<T> && Equals((StaticModInt<T>)obj);
        public bool Equals(StaticModInt<T> other) => Value == other.Value;
        public override int GetHashCode() => _v.GetHashCode();
    }

    /// <summary>
    /// 四則演算時に自動で mod を取る整数型。実行時に mod が決まる場合でも使用可能です。
    /// </summary>
    /// <remarks>
    /// 使用前に DynamicModInt&lt;<typeparamref name="T"/>&gt;.Mod に mod の値を設定する必要があります。
    /// </remarks>
    /// <typeparam name="T">mod の ID を表す構造体</typeparam>
    /// <example>
    /// <code>
    /// using AtCoder.ModInt = AtCoder.DynamicModInt&lt;AtCoder.ModID0&gt;;
    ///
    /// void SomeMethod()
    /// {
    ///     ModInt.Mod = 1000000009;
    ///     var m = new ModInt(1);
    ///     m -= 2;
    ///     Console.WriteLine(m);   // 1000000008
    /// }
    /// </code>
    /// </example>
    public readonly struct DynamicModInt<T> : IEquatable<DynamicModInt<T>> where T : struct, IDynamicModID
    {
        private readonly uint _v;
        private static Barrett bt;

        /// <summary>
        /// 格納されている値を返します。
        /// </summary>
        public int Value => (int)_v;

        /// <summary>
        /// mod を返します。
        /// </summary>
        public static int Mod
        {
            get => (int)bt.Mod;
            set
            {
                Debug.Assert(1 <= value);
                bt = new Barrett((uint)value);
            }
        }

        /// <summary>
        /// <paramref name="v"/> に対して mod を取らずに DynamicModInt&lt;<typeparamref name="T"/>&gt; 型のインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// <para>定数倍高速化のための関数です。 <paramref name="v"/> に 0 未満または mod 以上の値を入れた場合の挙動は未定義です。</para>
        /// <para>制約: 0≤|<paramref name="v"/>|&lt;mod</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> Raw(int v)
        {
            var u = unchecked((uint)v);
            Debug.Assert(bt != null, $"使用前に {nameof(DynamicModInt<T>)}<{nameof(T)}>.{nameof(Mod)} プロパティに mod の値を設定してください。");
            Debug.Assert(u < Mod);
            return new DynamicModInt<T>(u);
        }

        /// <summary>
        /// DynamicModInt&lt;<typeparamref name="T"/>&gt; 型のインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// <para>- 使用前に DynamicModInt&lt;<typeparamref name="T"/>&gt;.Mod に mod の値を設定する必要があります。</para>
        /// <para>- <paramref name="v"/> が 0 未満、もしくは mod 以上の場合、自動で mod を取ります。</para>
        /// </remarks>
        public DynamicModInt(long v) : this(Round(v)) { }

        private DynamicModInt(uint v) => _v = v;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Round(long v)
        {
            Debug.Assert(bt != null, $"使用前に {nameof(DynamicModInt<T>)}<{nameof(T)}>.{nameof(Mod)} プロパティに mod の値を設定してください。");
            var x = v % bt.Mod;
            if (x < 0)
            {
                x += bt.Mod;
            }
            return (uint)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> operator ++(DynamicModInt<T> value)
        {
            var v = value._v + 1;
            if (v == bt.Mod)
            {
                v = 0;
            }
            return new DynamicModInt<T>(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> operator --(DynamicModInt<T> value)
        {
            var v = value._v;
            if (v == 0)
            {
                v = bt.Mod;
            }
            return new DynamicModInt<T>(v - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> operator +(DynamicModInt<T> lhs, DynamicModInt<T> rhs)
        {
            var v = lhs._v + rhs._v;
            if (v >= bt.Mod)
            {
                v -= bt.Mod;
            }
            return new DynamicModInt<T>(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> operator -(DynamicModInt<T> lhs, DynamicModInt<T> rhs)
        {
            unchecked
            {
                var v = lhs._v - rhs._v;
                if (v >= bt.Mod)
                {
                    v += bt.Mod;
                }
                return new DynamicModInt<T>(v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DynamicModInt<T> operator *(DynamicModInt<T> lhs, DynamicModInt<T> rhs)
        {
            uint z = bt.Mul(lhs._v, rhs._v);
            return new DynamicModInt<T>(z);
        }

        /// <summary>
        /// 除算を行います。
        /// </summary>
        /// <remarks>
        /// <para>- 制約: <paramref name="rhs"/> に乗法の逆元が存在する。（gcd(<paramref name="rhs"/>, mod) = 1）</para>
        /// <para>- 計算量: O(log(mod))</para>
        /// </remarks>
        public static DynamicModInt<T> operator /(DynamicModInt<T> lhs, DynamicModInt<T> rhs) => lhs * rhs.Inverse();

        public static DynamicModInt<T> operator +(DynamicModInt<T> value) => value;
        public static DynamicModInt<T> operator -(DynamicModInt<T> value) => new DynamicModInt<T>() - value;
        public static bool operator ==(DynamicModInt<T> lhs, DynamicModInt<T> rhs) => lhs._v == rhs._v;
        public static bool operator !=(DynamicModInt<T> lhs, DynamicModInt<T> rhs) => lhs._v != rhs._v;
        public static implicit operator DynamicModInt<T>(int value) => new DynamicModInt<T>(value);
        public static implicit operator DynamicModInt<T>(long value) => new DynamicModInt<T>(value);

        /// <summary>
        /// 自身を x として、x^<paramref name="n"/> を返します。
        /// </summary>
        /// <remarks>
        /// <para>制約: 0≤|<paramref name="n"/>|</para>
        /// <para>計算量: O(log(<paramref name="n"/>))</para>
        /// </remarks>
        public DynamicModInt<T> Pow(long n)
        {
            Debug.Assert(0 <= n);
            var x = this;
            var r = Raw(1);

            while (n > 0)
            {
                if ((n & 1) > 0)
                {
                    r *= x;
                }
                x *= x;
                n >>= 1;
            }

            return r;
        }

        /// <summary>
        /// 自身を x として、 xy≡1 なる y を返します。
        /// </summary>
        /// <remarks>
        /// <para>制約: gcd(x, mod) = 1</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DynamicModInt<T> Inverse()
        {
            var (g, x) = InternalMath.InvGCD(_v, bt.Mod);
            Debug.Assert(g == 1);
            return new DynamicModInt<T>(x);
        }

        public override string ToString() => _v.ToString();
        public override bool Equals(object obj) => obj is DynamicModInt<T> && Equals((DynamicModInt<T>)obj);
        public bool Equals(DynamicModInt<T> other) => Value == other.Value;
        public override int GetHashCode() => _v.GetHashCode();
    }

    /// <summary>
    /// Fast moduler by barrett reduction
    /// <seealso href="https://en.wikipedia.org/wiki/Barrett_reduction"/>
    /// </summary>
    public class Barrett
    {
        public uint Mod { get; private set; }
        private ulong IM;
        public Barrett(uint m)
        {
            Mod = m;
            IM = unchecked((ulong)-1) / m + 1;
        }

        /// <summary>
        /// <paramref name="a"/> * <paramref name="b"/> mod m
        /// </summary>
        public uint Mul(uint a, uint b)
        {
            ulong z = a;
            z *= b;
            if (!Bmi2.X64.IsSupported) return (uint)(z % Mod);
            var x = Bmi2.X64.MultiplyNoFlags(z, IM);
            var v = unchecked((uint)(z - x * Mod));
            if (Mod <= v) v += Mod;
            return v;
        }
    }

    public static class InternalMath
    {
        /// <summary>
        /// g=gcd(a,b),xa=g(mod b) となるような 0≤x&lt;b/g の(g, x)
        /// </summary>
        /// <remarks>
        /// <para>制約: 1≤<paramref name="b"/></para>
        /// </remarks>
        public static (long, long) InvGCD(long a, long b)
        {
            a = SafeMod(a, b);
            if (a == 0) return (b, 0);

            long s = b, t = a;
            long m0 = 0, m1 = 1;

            long u;
            while (true)
            {
                if (t == 0)
                {
                    if (m0 < 0) m0 += b / s;
                    return (s, m0);
                }
                u = s / t;
                s -= t * u;
                m0 -= m1 * u;

                if (s == 0)
                {
                    if (m1 < 0) m1 += b / t;
                    return (t, m1);
                }
                u = t / s;
                t -= s * u;
                m1 -= m0 * u;
            }
        }

        public static long SafeMod(long x, long m)
        {
            x %= m;
            if (x < 0) x += m;
            return x;
        }
    }

    public class ModCombination<T> where T : struct, IStaticMod
    {
        readonly StaticModInt<T>[] _factorials;
        readonly StaticModInt<T>[] _invFactorials;

        public ModCombination(int max = 1000000)
        {
            if (max >= default(T).Mod)
            {
                ThrowArgumentOutOfRangeException();
            }

            _factorials = new StaticModInt<T>[max + 1];
            _invFactorials = new StaticModInt<T>[max + 1];

            _factorials[0] = _factorials[1] = StaticModInt<T>.Raw(1);
            _invFactorials[0] = _invFactorials[1] = StaticModInt<T>.Raw(1);

            for (int i = 2; i < _factorials.Length; i++)
            {
                _factorials[i] = _factorials[i - 1] * StaticModInt<T>.Raw(i);
            }

            _invFactorials[^1] = _factorials[^1].Inverse();

            for (int i = _invFactorials.Length - 2; i >= 0; i--)
            {
                _invFactorials[i] = _invFactorials[i + 1] * StaticModInt<T>.Raw(i + 1);
            }
        }

        public StaticModInt<T> Factorial(int n) => _factorials[n];

        public StaticModInt<T> Permutation(int n, int k) => _factorials[n] * _invFactorials[n - k];

        public StaticModInt<T> Combination(int n, int k) => _factorials[n] * _invFactorials[k] * _invFactorials[n - k];

        public StaticModInt<T> CombinationWithRepetition(int n, int k) => Combination(n + k - 1, k);

        public void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();
    }

    #endregion

    public class ModMatrix<T> where T : struct, IStaticMod
    {
        readonly StaticModInt<T>[] _values;
        public int Height { get; }
        public int Width { get; }

        public Span<StaticModInt<T>> this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _values.AsSpan(row * Width, Width);
        }

        public StaticModInt<T> this[int row, int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (unchecked((uint)row) >= Height)
                    ThrowsArgumentOutOfRangeException(nameof(row));
                else if (unchecked((uint)column) >= Width)
                    ThrowsArgumentOutOfRangeException(nameof(column));
                return _values[row * Width + column];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)row) >= Height)
                    ThrowsArgumentOutOfRangeException(nameof(row));
                else if (unchecked((uint)column) >= Width)
                    ThrowsArgumentOutOfRangeException(nameof(column));
                _values[row * Width + column] = value;
            }
        }

        public ModMatrix(int n) : this(n, n) { }

        public ModMatrix(int height, int width)
        {
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            Height = height;
            Width = width;
            _values = new StaticModInt<T>[height * width];
        }

        public ModMatrix(StaticModInt<T>[][] values) : this(values.Length, values[0].Length)
        {
            for (int row = 0; row < Height; row++)
            {
                if (Width != values[row].Length)
                    throw new ArgumentException($"{nameof(values)}の列数は揃っている必要があります。");
                var span = _values.AsSpan(row * Width, Width);
                values[row].AsSpan().CopyTo(span);
            }
        }

        public ModMatrix(StaticModInt<T>[,] values) : this(values.GetLength(0), values.GetLength(1))
        {
            for (int row = 0; row < Height; row++)
            {
                var span = _values.AsSpan(row * Width, Width);
                for (int column = 0; column < span.Length; column++)
                {
                    span[column] = values[row, column];
                }
            }
        }

        public ModMatrix(ModMatrix<T> matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;
            _values = new StaticModInt<T>[matrix._values.Length];
            matrix._values.AsSpan().CopyTo(_values);
        }

        public static ModMatrix<T> GetIdentity(int dimension)
        {
            var result = new ModMatrix<T>(dimension);
            for (int i = 0; i < dimension; i++)
            {
                result._values[i * result.Width + i] = 1;
            }
            return result;
        }

        public static ModMatrix<T> operator +(ModMatrix<T> a, ModMatrix<T> b)
        {
            CheckSameShape(a, b);

            var result = new ModMatrix<T>(a.Height, a.Width);
            for (int i = 0; i < result._values.Length; i++)
            {
                result._values[i] = a._values[i] + b._values[i];
            }
            return result;
        }

        public static ModMatrix<T> operator -(ModMatrix<T> a, ModMatrix<T> b)
        {
            CheckSameShape(a, b);

            var result = new ModMatrix<T>(a.Height, a.Width);
            for (int i = 0; i < result._values.Length; i++)
            {
                result._values[i] = a._values[i] - b._values[i];
            }
            return result;
        }

        public static ModMatrix<T> operator *(ModMatrix<T> a, ModMatrix<T> b)
        {
            if (a.Width != b.Height)
                throw new ArgumentException($"{nameof(a)}の列数と{nameof(b)}の行数は等しくなければなりません。");

            var result = new ModMatrix<T>(a.Height, b.Width);
            for (int i = 0; i < result.Height; i++)
            {
                var aSpan = a._values.AsSpan(i * a.Width, a.Width);
                var resultSpan = result._values.AsSpan(i * result.Width, result.Width);
                for (int k = 0; k < aSpan.Length; k++)
                {
                    var bSpan = b._values.AsSpan(k * b.Width, b.Width);
                    for (int j = 0; j < resultSpan.Length; j++)
                    {
                        resultSpan[j] += aSpan[k] * bSpan[j];
                    }
                }
            }
            return result;
        }

        public static ModVector<T> operator *(ModMatrix<T> matrix, ModVector<T> vector)
        {
            if (matrix.Width != vector.Length)
                throw new ArgumentException($"{nameof(matrix)}の列数と{nameof(vector)}の行数は等しくなければなりません。");

            var result = new ModVector<T>(vector.Length);
            for (int i = 0; i < result.Length; i++)
            {
                var matrixSpan = matrix[i];
                for (int k = 0; k < matrixSpan.Length; k++)
                {
                    result[i] += matrixSpan[k] * vector[k];
                }
            }
            return result;
        }

        public ModMatrix<T> Pow(long pow)
        {
            if (Height != Width)
                throw new ArgumentException("累乗を行う行列は正方行列である必要があります。");
            if (pow < 0)
                throw new ArgumentException($"{nameof(pow)}は0以上の整数である必要があります。");

            var powMatrix = new ModMatrix<T>(this);
            var result = GetIdentity(Height);
            while (pow > 0)
            {
                if ((pow & 1) > 0)
                {
                    result *= powMatrix;
                }
                powMatrix *= powMatrix;
                pow >>= 1;
            }
            return result;
        }

        private static void CheckSameShape(ModMatrix<T> a, ModMatrix<T> b)
        {
            if (a.Height != b.Height)
                throw new ArgumentException($"{nameof(a)}の行数と{nameof(b)}の行数は等しくなければなりません。");
            else if (a.Width != b.Width)
                throw new ArgumentException($"{nameof(a)}の列数と{nameof(b)}の列数は等しくなければなりません。");
        }

        private void ThrowsArgumentOutOfRangeException(string paramName) => throw new ArgumentOutOfRangeException(paramName);
        public override string ToString() => $"({Height}x{Width})matrix";
    }

    public class ModVector<T> where T : struct, IStaticMod
    {
        readonly StaticModInt<T>[] _values;
        public int Length => _values.Length;

        public ModVector(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            _values = new StaticModInt<T>[length];
        }

        public ModVector(ReadOnlySpan<StaticModInt<T>> vector)
        {
            _values = new StaticModInt<T>[vector.Length];
            vector.CopyTo(_values);
        }

        public ModVector(ModVector<T> vector) : this(vector._values) { }

        public StaticModInt<T> this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }

        public static ModVector<T> operator +(ModVector<T> a, ModVector<T> b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = new ModVector<T>(a.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }

        public static ModVector<T> operator -(ModVector<T> a, ModVector<T> b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = new ModVector<T>(a.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        public static StaticModInt<T> operator *(ModVector<T> a, ModVector<T> b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = StaticModInt<T>.Zero;
            for (int i = 0; i < a.Length; i++)
            {
                result += a[i] * b[i];
            }
            return result;
        }

        public override string ToString() => $"({Length})vector";
    }

    public class Eratosthenes
    {
        /// <summary>
        /// Smallest Prime Factorを保存した配列
        /// </summary>
        readonly int[] _spf;

        public Eratosthenes(int max)
        {
            _spf = Enumerable.Range(0, max + 1).ToArray();
            for (int i = 2; i * i <= max; i++)
            {
                if (_spf[i] == i)
                {
                    for (int mul = i << 1; mul <= max; mul += i)
                    {
                        if (_spf[mul] == mul)
                        {
                            _spf[mul] = i;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrime(int n) => n >= 2 && _spf[n] == n;

        public IEnumerable<(int prime, int count)> PrimeFactorize(int n)
        {
            if (n <= 0 || _spf.Length <= n)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は[0, {_spf.Length})の間でなければなりません");
            }
            else if (n == 1)
            {
                yield break;
            }
            else
            {
                var last = _spf[n];
                var streak = 0;
                while (n > 1)
                {
                    if (_spf[n] == last)
                    {
                        streak++;
                    }
                    else
                    {
                        yield return (last, streak);
                        last = _spf[n];
                        streak = 1;
                    }

                    n /= last;
                }
                yield return (last, streak);
            }
        }

        public IEnumerable<int> GetDivisiors(int n)
        {
            if (n <= 0 || _spf.Length <= n)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は[0, {_spf.Length})の間でなければなりません");
            }
            else
            {
                var primes = PrimeFactorize(n).ToArray();
                return GetDivisiors(primes, 0);
            }
        }

        IEnumerable<int> GetDivisiors((int prime, int count)[] primes, int depth)
        {
            if (depth == primes.Length)
            {
                yield return 1;
            }
            else
            {
                var current = 1;
                var children = GetDivisiors(primes, depth + 1).ToArray();

                foreach (var child in children)
                {
                    yield return child;
                }

                for (int i = 0; i < primes[depth].count; i++)
                {
                    current *= primes[depth].prime;
                    foreach (var child in children)
                    {
                        yield return current * child;
                    }
                }
            }
        }
    }

    public interface ISemigroup<TSet>
    {
        public TSet Merge(TSet other);
    }

    public interface IMonoid<TSet> : ISemigroup<TSet>
    {
        public TSet Identity { get; }
    }

    public interface IMonoidWithAct<TMonoid, TOperator> : IMonoid<TOperator>
        where TMonoid : IMonoid<TMonoid>
        where TOperator : IMonoid<TOperator>
    {
        public TMonoid Act(TMonoid monoid);
    }

    public interface IGroup<TSet> : IMonoid<TSet> 
    {
        public TSet Invert();
        public static TSet operator ~(IGroup<TSet> a) => a.Invert();
    }
}

namespace AtCoderTemplateForNetCore.Algorithms
{
    public static class ZAlgorithm
    {
        public static int[] SearchAll(string s) => SearchAll(s.AsSpan());

        public static int[] SearchAll<T>(ReadOnlySpan<T> s) where T : IEquatable<T>
        {
            var z = new int[s.Length];
            z[0] = s.Length;
            var offset = 1;
            var length = 0;

            while (offset < s.Length)
            {
                while (offset + length < s.Length && s[length].Equals(s[offset + length]))
                {
                    length++;
                }
                z[offset] = length;

                if (length == 0)
                {
                    offset++;
                    continue;
                }

                int copyLength = 1;
                while (copyLength < length && copyLength + z[copyLength] < length)
                {
                    z[offset + copyLength] = z[copyLength];
                    copyLength++;
                }
                offset += copyLength;
                length -= copyLength;
            }

            return z;
        }
    }

    /// <summary>
    /// MP法（文字列検索アルゴリズム）
    /// </summary>
    public class MorrisPratt<T> where T : IEquatable<T>
    {
        readonly T[] _searchSequence;
        readonly int[] _matchLength;

        public ReadOnlySpan<T> SearchSequence => _searchSequence.AsSpan();

        /// <summary>
        /// 検索データ列の前処理を行います。
        /// </summary>
        /// <param name="searchSequence">検索データ列</param>
        public MorrisPratt(ReadOnlySpan<T> searchSequence)
        {
            _searchSequence = searchSequence.ToArray();
            _matchLength = new int[_searchSequence.Length + 1];
            _matchLength[0] = -1;
            int j = -1;
            for (int i = 0; i < _searchSequence.Length; i++)
            {
                while (j != -1 && !_searchSequence[j].Equals(_searchSequence[i]))
                {
                    j = _matchLength[j];
                }
                j++;
                _matchLength[i + 1] = j;
            }
        }

        /// <summary>
        /// 与えられた対象データ列の部分列のうち、検索データ列にマッチする部分列の開始インデックスを取得します。
        /// </summary>
        /// <param name="targetSequence">検索対象データ列</param>
        /// <returns></returns>
        public List<int> SearchAll(ReadOnlySpan<T> targetSequence)
        {
            var results = new List<int>();
            int j = 0;
            for (int i = 0; i < targetSequence.Length; i++)
            {
                while (j != -1 && !_searchSequence[j].Equals(targetSequence[i]))
                {
                    j = _matchLength[j];
                }
                j++;
                if (j == _searchSequence.Length)
                {
                    results.Add(i - j + 1);
                    j = _matchLength[j];
                }
            }
            return results;
        }
    }

    /// <summary>
    /// 参考: https://qiita.com/keymoon/items/11fac5627672a6d6a9f6
    /// ジェネリクスに対応させるにはGetHashCode()を足していく？実装によっては重そうなのでとりあえずパス。
    /// </summary>
    public class RollingHash
    {
        const ulong Mask30 = (1UL << 30) - 1;
        const ulong Mask31 = (1UL << 31) - 1;
        const ulong Mod = (1UL << 61) - 1;
        const ulong Positivizer = Mod * ((1UL << 3) - 1);   // 引き算する前に足すことでmodが負になることを防ぐやつ
        static readonly uint base1;
        static readonly uint base2;
        static readonly List<ulong> pow1;
        static readonly List<ulong> pow2;

        static RollingHash()
        {
            var random = new Random();
            base1 = (uint)random.Next(129, int.MaxValue >> 2);
            base2 = (uint)random.Next(int.MaxValue >> 2, int.MaxValue); // 32bit目は0にしておく
            pow1 = new List<ulong>() { 1 };
            pow2 = new List<ulong>() { 1 };
        }

        ulong[] hash1;
        ulong[] hash2;
        public string RawString { get; }
        public int Length => RawString.Length;

        public RollingHash(string s)
        {
            RawString = s;
            hash1 = new ulong[s.Length + 1];
            hash2 = new ulong[s.Length + 1];

            for (int i = pow1.Count; i < s.Length + 1; i++)
            {
                pow1.Add(CalculateModular(Multiply(pow1[i - 1], base1)));
                pow2.Add(CalculateModular(Multiply(pow2[i - 1], base2)));
            }

            for (int i = 0; i < s.Length; i++)
            {
                hash1[i + 1] = CalculateModular(Multiply(hash1[i], base1) + s[i]);
                hash2[i + 1] = CalculateModular(Multiply(hash2[i], base2) + s[i]);
            }
        }

        public (ulong, ulong) this[Range range]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var (offset, length) = range.GetOffsetAndLength(Length);
                return Slice(offset, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (ulong, ulong) Slice(int begin, int length)
        {
            var result1 = CalculateModular(hash1[begin + length] + Positivizer - Multiply(hash1[begin], pow1[length]));
            var result2 = CalculateModular(hash2[begin + length] + Positivizer - Multiply(hash2[begin], pow2[length]));
            return (result1, result2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Multiply(ulong l, ulong r)
        {
            var lu = l >> 31;
            var ll = l & Mask31;
            var ru = r >> 31;
            var rl = r & Mask31;
            var mid = ll * ru + lu * rl;
            return ((lu * ru) << 1) + ll * rl + ((mid & Mask30) << 31) + (mid >> 30);   // a * 2^61 ≡ a (mod 2^61 - 1)を使う
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Multiply(ulong l, uint r)
        {
            var lu = l >> 31;
            var mid = lu * r;
            return (l & Mask31) * r + ((mid & Mask30) << 31) + (mid >> 30); // rの32bit目は0としている
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong CalculateModular(ulong value)
        {
            value = (value & Mod) + (value >> 61);
            if (value >= Mod)
            {
                value -= Mod;
            }
            return value;
        }

        public override string ToString() => RawString;
    }

    public class XorShift
    {
        ulong _x;

        public XorShift() : this((ulong)DateTime.Now.Ticks) { }

        public XorShift(ulong seed)
        {
            _x = seed;
        }

        /// <summary>
        /// [0, (2^64)-1)の乱数を生成します。
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Next()
        {
            _x = _x ^ (_x << 13);
            _x = _x ^ (_x >> 7);
            _x = _x ^ (_x << 17);
            return _x;
        }

        /// <summary>
        /// [0, <c>exclusiveMax</c>)の乱数を生成します。
        /// </summary>
        /// <param name="exclusiveMax"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int exclusiveMax) => (int)(Next() % (uint)exclusiveMax);

        /// <summary>
        /// [0, <c>exclusiveMax</c>)の乱数を生成します。
        /// </summary>
        /// <param name="exclusiveMax"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Next(long exclusiveMax) => (long)(Next() % (ulong)exclusiveMax);

        /// <summary>
        /// [0.0, 1.0)の乱数を生成します。
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble()
        {
            const ulong max = 1UL << 50;
            const ulong mask = max - 1;
            return (double)(Next() & mask) / max;
        }
    }

    public static class AlgorithmHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateWhenSmall<T>(ref T value, T other) where T : IComparable<T>
        {
            if (other.CompareTo(value) < 0)
            {
                value = other;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateWhenLarge<T>(ref T value, T other) where T : IComparable<T>
        {
            if (other.CompareTo(value) > 0)
            {
                value = other;
            }
        }
    }

    public class CompressedCoordinate<T> where T : IComparable<T>, IEquatable<T>
    {
        readonly int[] _compressed;
        readonly T[] _expander;
        public int Count => _expander.Length;

        public CompressedCoordinate(ReadOnlySpan<T> data)
        {
            _expander = data.ToArray().Distinct().ToArray();
            Array.Sort(_expander);

            _compressed = new int[data.Length];
            var span = _expander.AsSpan();
            for (int i = 0; i < _compressed.Length; i++)
            {
                _compressed[i] = span.BinarySearch(data[i]);
            }
        }

        public int this[int index] => _compressed[index];
        public T Expand(int compressedValue) => _expander[compressedValue];
    }
}

#endregion

#region Collections

namespace AtCoderTemplateForNetCore.Collections
{
    public class UnionFind
    {
        private int[] _parentsOrSizes;
        public int Count => _parentsOrSizes.Length;
        public int Groups { get; private set; }

        public UnionFind(int count)
        {
            _parentsOrSizes = new int[count];
            _parentsOrSizes.AsSpan().Fill(-1);
            Groups = _parentsOrSizes.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Unite(int a, int b)
        {
            var x = GetLeader(a);
            var y = GetLeader(b);

            if (x == y)
            {
                return false;
            }
            else
            {
                if (-_parentsOrSizes[x] < _parentsOrSizes[y])
                {
                    (x, y) = (y, x);
                }

                _parentsOrSizes[x] += _parentsOrSizes[y];
                _parentsOrSizes[y] = x;
                Groups--;
                return true;
            }
        }

        public bool IsInSameGroup(int a, int b) => GetLeader(a) == GetLeader(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLeader(int index)
        {
            if (index >= _parentsOrSizes.Length)
            {
                ThrowArgumentException();
            }

            return GetLeaderRecursive(index);

            int GetLeaderRecursive(int index)
            {
                if (unchecked((uint)_parentsOrSizes[index]) < _parentsOrSizes.Length)
                {
                    return _parentsOrSizes[index] = GetLeaderRecursive(_parentsOrSizes[index]);
                }
                else
                {
                    return index;
                }
            }
        }

        public int GetGroupSize(int index) => -_parentsOrSizes[GetLeader(index)];

        public int[][] GetAllGroups()
        {
            var resultIndices = ArrayPool<int>.Shared.Rent(_parentsOrSizes.Length);
            var index = 0;
            var results = new int[Groups][];

            for (int i = 0; i < _parentsOrSizes.Length; i++)
            {
                if (_parentsOrSizes[i] < 0)
                {
                    results[index] = new int[-_parentsOrSizes[i]];
                    resultIndices[i] = index++;
                }
            }

            var counts = new int[results.Length];

            for (int i = 0; i < _parentsOrSizes.Length; i++)
            {
                var group = resultIndices[GetLeader(i)];
                results[group][counts[group]++] = i;
            }

            ArrayPool<int>.Shared.Return(resultIndices);

            return results;
        }

        private void ThrowArgumentException() => throw new ArgumentException();
    }

    public class Deque<T> : IReadOnlyCollection<T>
    {
        public int Count { get; private set; }
        private T[] _data;
        private int _first;
        private int _mask;

        public Deque() : this(4) { }

        public Deque(int minCapacity)
        {
            if (minCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minCapacity), $"{nameof(minCapacity)}は0より大きい値でなければなりません。");
            }
            var capacity = GetPow2Over(minCapacity);
            _data = new T[capacity];
            _first = 0;
            _mask = capacity - 1;
        }

        public Deque(IEnumerable<T> collection)
        {
            var dataArray = collection.ToArray();
            var capacity = GetPow2Over(dataArray.Length);
            _data = new T[capacity];
            _first = 0;
            _mask = capacity - 1;

            for (int i = 0; i < dataArray.Length; i++)
            {
                _data[i] = dataArray[i];
                Count++;
            }
        }

        public T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var offset = index.GetOffset(Count);
                if (unchecked((uint)offset) >= Count)
                {
                    ThrowArgumentOutOfRangeException(nameof(index), $"{nameof(index)}がコレクションの範囲外です。");
                }
                return _data[(_first + offset) & _mask];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueFirst(T item)
        {
            if (_data.Length == Count)
            {
                Resize();
            }

            _first = (_first - 1) & _mask;
            _data[_first] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueLast(T item)
        {
            if (_data.Length == Count)
            {
                Resize();
            }

            _data[(_first + Count++) & _mask] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DequeueFirst()
        {
            if (Count == 0)
            {
                ThrowInvalidOperationException("Queueが空です。");
            }

            var value = _data[_first];
            _data[_first++] = default;
            _first &= _mask;
            Count--;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DequeueLast()
        {
            if (Count == 0)
            {
                ThrowInvalidOperationException("Queueが空です。");
            }

            var index = (_first + --Count) & _mask;
            var value = _data[index];
            _data[index] = default;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekFirst()
        {
            if (Count == 0)
            {
                ThrowInvalidOperationException("Queueが空です。");
            }

            return _data[_first];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekLast()
        {
            if (Count == 0)
            {
                ThrowInvalidOperationException("Queueが空です。");
            }

            return _data[(_first + Count - 1) & _mask];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize()
        {
            var newArray = new T[_data.Length << 1];
            var span = _data.AsSpan();
            var firstHalf = span[_first..];
            var lastHalf = span[.._first];
            firstHalf.CopyTo(newArray);
            lastHalf.CopyTo(newArray.AsSpan(firstHalf.Length));
            _data = newArray;
            _first = 0;
            _mask = _data.Length - 1;
        }

        private void ThrowArgumentOutOfRangeException(string paramName, string message) => throw new ArgumentOutOfRangeException(paramName, message);
        private void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);

        private int GetPow2Over(int n)
        {
            n--;
            var result = 1;
            while (n != 0)
            {
                n >>= 1;
                result <<= 1;
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                var offset = (_first + i) & _mask;
                yield return _data[offset];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        const int InitialSize = 4;
        private readonly int _reverseFactor;
        private T[] _heap;
        public int Count { get; private set; }
        public bool IsDescending => _reverseFactor == 1;

        public PriorityQueue(Order order)
        {
            _reverseFactor = order == Order.Ascending ? -1 : 1;
            _heap = new T[InitialSize];
            Count = 0;
        }

        public PriorityQueue(Order order, IEnumerable<T> collection) : this(order)
        {
            foreach (var item in collection)
            {
                Enqueue(item);
            }
        }

        public void Enqueue(T item)
        {
            if (Count >= _heap.Length)
            {
                var temp = new T[_heap.Length << 1];
                _heap.AsSpan().CopyTo(temp);
                _heap = temp;
            }

            var index = Count++;
            ref var child = ref _heap[index];
            child = item;

            while (index > 0)
            {
                index = (index - 1) >> 1;
                ref var parent = ref _heap[index];

                if (Compare(child, parent) <= 0)
                {
                    break;
                }

                Swap(ref child, ref parent);
                child = ref parent;
            }
        }

        public T Dequeue()
        {
            var index = 0;
            ref var parent = ref _heap[index];
            var item = parent;
            parent = _heap[--Count];
            var span = _heap.AsSpan(0, Count);

            while (true)
            {
                index = (index << 1) + 1;

                if (unchecked((uint)index < (uint)span.Length))
                {
                    ref var child = ref span[index];
                    var r = index + 1;

                    if (unchecked((uint)r < (uint)span.Length))
                    {
                        ref var brother = ref span[r];
                        if (Compare(child, brother) < 0)
                        {
                            child = ref brother;
                            index = r;
                        }
                    }

                    if (Compare(parent, child) < 0)
                    {
                        Swap(ref parent, ref child);
                        parent = ref child;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return item;
        }

        public T Peek() => _heap[0];

        public void Clear() => Count = 0;

        private int Compare(T a, T b) => _reverseFactor * a.CompareTo(b);

        private void Swap(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var copy = new T[_heap.Length];
            var cnt = Count;
            _heap.AsSpan().CopyTo(copy);

            try
            {
                while (Count > 0)
                {
                    yield return Dequeue();
                }
            }
            finally
            {
                _heap = copy;
                Count = cnt;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public enum Order
        {
            Ascending,
            Descending
        }
    }

    /// <summary>
    /// SWAG
    /// </summary>
    public class SlidingWindowAggregation<T> where T : ISemigroup<T>
    {
        private readonly Stack<T> _frontStack;
        private readonly Stack<(T value, T aggregation)> _backStack;
        public int Count => _frontStack.Count + _backStack.Count;

        public SlidingWindowAggregation()
        {
            _frontStack = new Stack<T>();
            _backStack = new Stack<(T value, T aggregation)>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T QueryAll()
        {
            if (_frontStack.Count == 0)
            {
                Move();
            }

            // _frontStackが空のときは死ぬということに約束する
            if (_backStack.Count == 0)
            {
                return _frontStack.Peek();
            }
            else
            {
                return _frontStack.Peek().Merge(_backStack.Peek().aggregation);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T value)
        {
            if (_backStack.Count > 0)
            {
                _backStack.Push((value, _backStack.Peek().aggregation.Merge(value)));
            }
            else
            {
                _backStack.Push((value, value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dequeue()
        {
            if (_frontStack.Count == 0)
            {
                Move();
            }
            _frontStack.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Move()
        {
            while (_backStack.Count > 0)
            {
                var (value, _) = _backStack.Pop();

                if (_frontStack.Count == 0)
                {
                    _frontStack.Push(value);
                }
                else
                {
                    _frontStack.Push(value.Merge(_frontStack.Peek()));
                }
            }
        }
    }

    readonly struct MinInt : IMonoid<MinInt>
    {
        public readonly int Value;
        public MinInt Identity => new MinInt(int.MaxValue);
        public MinInt(int value) => Value = value;

        public MinInt Merge(MinInt other) => Value < other.Value ? this : other;
        public override string ToString() => Value.ToString();
        public static implicit operator int(MinInt value) => value.Value;
    }

    readonly struct MaxInt : IMonoid<MaxInt>
    {
        public readonly int Value;
        public MaxInt Identity => new MaxInt(int.MinValue);
        public MaxInt(int value) => Value = value;

        public MaxInt Merge(MaxInt other) => Value > other.Value ? this : other;
        public override string ToString() => Value.ToString();
        public static implicit operator int(MaxInt value) => value.Value;
    }

    public class SegmentTree<T> where T : struct, IMonoid<T>
    {
        // 1-indexed
        protected readonly T[] _data;

        public int Length { get; }
        protected Span<T> Leaves => _data.AsSpan(HalfLength, Length);
        protected int HalfLength => _data.Length >> 1;

        /// <summary>
        /// 単位元で初期化します。
        /// </summary>
        public SegmentTree(int n) : this(n, default(T).Identity) { }

        /// <summary>
        /// 指定した値で初期化します。
        /// </summary>
        public SegmentTree(int n, T initialValue)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Length = n;
            _data = new T[1 << (CeilPow2(n) + 1)];
            _data.AsSpan(HalfLength, Length).Fill(initialValue);
            Build();
        }

        /// <summary>
        /// 指定したデータ列で初期化します。
        /// </summary>
        public SegmentTree(ReadOnlySpan<T> values)
        {
            Length = values.Length;
            _data = new T[1 << (CeilPow2(values.Length) + 1)];
            values.CopyTo(Leaves);
            Build();
        }

        public virtual T this[int index]
        {
            get => Leaves[index];
            set
            {
                Leaves[index] = value;
                index += HalfLength;
                while ((index >>= 1) > 0)
                {
                    _data[index] = _data[(index << 1) + 0].Merge(_data[(index << 1) + 1]);
                }
            }
        }

        public T Query(Range range) => Query(range.Start, range.End);

        public T Query(Index left, Index right) => Query(left.GetOffset(Length), right.GetOffset(Length));

        public virtual T Query(int left, int right)
        {
            if (unchecked((uint)left > (uint)Length || (uint)right > (uint)Length || left > right))
            {
                throw new ArgumentOutOfRangeException();
            }

            var sumL = default(T).Identity;
            var sumR = default(T).Identity;
            left += HalfLength;
            right += HalfLength;

            while (left < right)
            {
                if ((left & 1) > 0)
                {
                    sumL = sumL.Merge(_data[left++]);
                }
                if ((right & 1) > 0)
                {
                    sumR = _data[--right].Merge(sumR);
                }
                left >>= 1;
                right >>= 1;
            }

            return sumL.Merge(sumR);
        }

        public T QueryAll() => _data[1];

        private void Build()
        {
            var parents = HalfLength;
            _data.AsSpan(parents + Length).Fill(default(T).Identity);
            for (int i = parents - 1; i >= 0; i--)
            {
                _data[i] = _data[(i << 1) + 0].Merge(_data[(i << 1) + 1]);
            }
        }

        /// <summary>
        /// [l, r)が条件を満たす最大のrを求めます。
        /// </summary>
        public int FindMaxRight(Index left, Predicate<T> predicate) => FindMaxRight(left.GetOffset(Length), predicate);

        /// <summary>
        /// [l, r)が条件を満たす最大のrを求めます。
        /// </summary>
        public virtual int FindMaxRight(int left, Predicate<T> predicate)
        {
            // 単位元は条件式を満たす必要がある
            Debug.Assert(predicate(default(T).Identity));

            if (unchecked((uint)left > Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (left == Length)
            {
                return Length;
            }

            var right = left + HalfLength;
            var sum = default(T).Identity;

            do
            {
                right >>= BitOperations.TrailingZeroCount(right);
                var merged = sum.Merge(_data[right]);
                if (!predicate(merged))
                {
                    return DownSearch(right, sum, predicate);
                }

                sum = merged;
                right++;
            } while ((right & -right) != right);

            return Length;

            int DownSearch(int right, T sum, Predicate<T> predicate)
            {
                while (right < HalfLength)
                {
                    right <<= 1;
                    var merged = sum.Merge(_data[right]);
                    if (predicate(merged))
                    {
                        sum = merged;
                        right++;
                    }
                }
                return right - HalfLength;
            }
        }

        /// <summary>
        /// [l, r)が条件を満たす最小のlを求めます。
        /// </summary>
        public int FindMinLeft(Index right, Predicate<T> predicate) => FindMinLeft(right.GetOffset(Length), predicate);

        /// <summary>
        /// [l, r)が条件を満たす最小のlを求めます。
        /// </summary>
        public virtual int FindMinLeft(int right, Predicate<T> predicate)
        {
            // 単位元は条件式を満たす必要がある
            Debug.Assert(predicate(default(T).Identity));

            if (unchecked((uint)right > Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (right == 0)
            {
                return 0;
            }

            var left = right + HalfLength;
            var sum = default(T).Identity;

            do
            {
                left--;
                left >>= BitOperations.TrailingZeroCount((1 << BitOperations.Log2((uint)left)) | ~left);

                var merged = _data[left].Merge(sum);
                if (!predicate(merged))
                {
                    return DownSearch(left, sum, predicate);
                }

                sum = merged;
            } while ((left & -left) != left);

            return 0;

            int DownSearch(int left, T sum, Predicate<T> predicate)
            {
                while (left < HalfLength)
                {
                    left = (left << 1) + 1;
                    var merged = _data[left].Merge(sum);
                    if (predicate(merged))
                    {
                        sum = merged;
                        left--;
                    }
                }
                return left + 1 - HalfLength;
            }
        }

        protected static int CeilPow2(int n)
        {
            var m = (uint)n;
            if (m <= 1)
            {
                return 0;
            }
            else
            {
                return BitOperations.Log2(m - 1) + 1;
            }
        }
    }

    public class LazySegmentTree<TValue, TLazy> : SegmentTree<TValue>
        where TValue : struct, IMonoid<TValue>
        where TLazy : struct, IMonoidWithAct<TValue, TLazy>
    {
        // 1-indexed
        protected readonly TLazy[] _lazies;
        protected readonly bool[] _activated;
        private readonly int _log;

        /// <summary>
        /// 単位元で初期化します。
        /// </summary>
        public LazySegmentTree(int n) : this(n, default(TValue).Identity) { }

        /// <summary>
        /// 指定した値で初期化します。
        /// </summary>
        public LazySegmentTree(int n, TValue initialValue) : base(n, initialValue)
        {
            _lazies = new TLazy[_data.Length];
            _activated = new bool[_data.Length];
            _lazies.AsSpan().Fill(default(TLazy).Identity);
            _log = CeilPow2(n);
        }

        /// <summary>
        /// 指定したデータ列で初期化します。
        /// </summary>
        public LazySegmentTree(ReadOnlySpan<TValue> values) : base(values)
        {
            _lazies = new TLazy[_data.Length];
            _activated = new bool[_data.Length];
            _lazies.AsSpan().Fill(default(TLazy).Identity);
            _log = CeilPow2(values.Length);
        }

        public override TValue this[int index]
        {
            get
            {
                var i = index + HalfLength;
                for (int l = _log; l >= 1; l--)
                {
                    LazyEvaluate(i >> l);
                }

                return Leaves[index];
            }
            set
            {
                var i = index + HalfLength;
                for (int l = _log; l >= 1; l--)
                {
                    LazyEvaluate(i >> l);
                }

                Leaves[index] = value;

                for (int l = 1; l <= _log; l++)
                {
                    UpdateMonoid(i >> l);
                }
            }
        }

        public override TValue Query(int left, int right)
        {
            if (unchecked((uint)left > (uint)Length || (uint)right > (uint)Length || left > right))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (left == right)
            {
                return default(TValue).Identity;
            }

            var l = left + HalfLength;
            var r = right + HalfLength;

            for (int i = _log; i >= 1; i--)
            {
                // 末尾が..000となっているものは更新不要
                if (((l >> i) << i) != l)
                {
                    LazyEvaluate(l >> i);
                }
                if (((r >> i) << i) != r)
                {
                    LazyEvaluate(r >> i);
                }
            }

            return base.Query(left, right);
        }

        public void Apply(Index index, TLazy actor) => Apply(index.GetOffset(Length), actor);

        public void Apply(int index, TLazy actor)
        {
            if (unchecked((uint)index >= (uint)Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            index += HalfLength;

            for (int i = _log; i >= 1; i--)
            {
                LazyEvaluate(index >> i);
            }

            _data[index] = actor.Act(_data[index]);

            for (int i = 1; i <= _log; i++)
            {
                UpdateMonoid(index >> i);
            }
        }

        public void Apply(Range range, TLazy actor) => Apply(range.Start, range.End, actor);

        public void Apply(Index left, Index right, TLazy actor) => Apply(left.GetOffset(Length), right.GetOffset(Length), actor);

        public void Apply(int left, int right, TLazy actor)
        {
            if (unchecked((uint)left > (uint)Length || (uint)right > (uint)Length || left > right))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (left == right)
            {
                return;
            }

            var l = left + HalfLength;
            var r = right + HalfLength;

            for (int i = _log; i >= 1; i--)
            {
                // 末尾が..000となっているものは更新不要
                if (((l >> i) << i) != l)
                {
                    LazyEvaluate(l >> i);
                }
                if (((r >> i) << i) != r)
                {
                    LazyEvaluate((r - 1) >> i);
                }
            }

            while (l < r)
            {
                if ((l & 1) > 0)
                {
                    ApplyLazy(l++, actor);
                }
                if ((r & 1) > 0)
                {
                    ApplyLazy(--r, actor);
                }
                l >>= 1;
                r >>= 1;
            }

            l = left + HalfLength;
            r = right + HalfLength;

            for (int i = 1; i <= _log; i++)
            {
                if (((l >> i) << i) != l)
                {
                    UpdateMonoid(l >> i);
                }
                if (((r >> i) << i) != r)
                {
                    UpdateMonoid((r - 1) >> i);
                }
            }
        }

        /// <summary>
        /// [l, r)が条件を満たす最大のrを求めます。
        /// </summary>
        public override int FindMaxRight(int left, Predicate<TValue> predicate)
        {
            // 単位元は条件式を満たす必要がある
            Debug.Assert(predicate(default(TValue).Identity));

            if (unchecked((uint)left > Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (left == Length)
            {
                return Length;
            }

            var right = left + HalfLength;
            var sum = default(TValue).Identity;

            for (int i = _log; i >= 1; i--)
            {
                LazyEvaluate(right >> i);
            }

            do
            {
                right >>= BitOperations.TrailingZeroCount(right);
                var merged = sum.Merge(_data[right]);

                if (!predicate(merged))
                {
                    return DownSearch(right, sum, predicate);
                }

                sum = merged;
                right++;
            } while ((right & -right) != right);

            return Length;

            int DownSearch(int right, TValue sum, Predicate<TValue> predicate)
            {
                while (right < HalfLength)
                {
                    LazyEvaluate(right);
                    right <<= 1;
                    var merged = sum.Merge(_data[right]);
                    if (predicate(merged))
                    {
                        sum = merged;
                        right++;
                    }
                }
                return right - HalfLength;
            }
        }

        /// <summary>
        /// [l, r)が条件を満たす最小のlを求めます。
        /// </summary>
        public override int FindMinLeft(int right, Predicate<TValue> predicate)
        {
            // 単位元は条件式を満たす必要がある
            Debug.Assert(predicate(default(TValue).Identity));

            if (unchecked((uint)right > Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (right == 0)
            {
                return 0;
            }

            var left = right + HalfLength;
            var sum = default(TValue).Identity;

            for (int i = _log; i >= 1; i--)
            {
                LazyEvaluate((left - 1) >> i);
            }

            do
            {
                left--;
                left >>= BitOperations.TrailingZeroCount((1 << BitOperations.Log2((uint)left)) | ~left);

                var merged = _data[left].Merge(sum);
                if (!predicate(merged))
                {
                    return DownSearch(left, sum, predicate);
                }

                sum = merged;
            } while ((left & -left) != left);

            return 0;

            int DownSearch(int left, TValue sum, Predicate<TValue> predicate)
            {
                while (left < HalfLength)
                {
                    LazyEvaluate(left);
                    left = (left << 1) + 1;
                    var merged = _data[left].Merge(sum);
                    if (predicate(merged))
                    {
                        sum = merged;
                        left--;
                    }
                }
                return left + 1 - HalfLength;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMonoid(int index) => _data[index] = _data[(index << 1) + 0].Merge(_data[(index << 1) + 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyLazy(int index, TLazy actor)
        {
            _data[index] = actor.Act(_data[index]);
            // 自身が葉でない場合
            if (index < HalfLength)
            {
                _lazies[index] = actor.Merge(_lazies[index]);
                _activated[index] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LazyEvaluate(int index)
        {
            if (_activated[index])
            {
                ref var lazy = ref _lazies[index];
                ApplyLazy((index << 1) + 0, lazy);
                ApplyLazy((index << 1) + 1, lazy);
                lazy = default(TLazy).Identity;
                _activated[index] = false;
            }
        }
    }

    public class BinaryIndexedTree
    {
        long[] _data;
        public int Length { get; }

        public BinaryIndexedTree(int length)
        {
            _data = new long[length + 1];   // 内部的には1-indexedにする
            Length = length;
        }

        public BinaryIndexedTree(IEnumerable<long> data, int length) : this(length)
        {
            var count = 0;
            foreach (var n in data)
            {
                AddAt(count++, n);
            }
        }

        public BinaryIndexedTree(ICollection<long> collection) : this(collection, collection.Count) { }

        public long this[int index]
        {
            get => Sum(index, index + 1);
            set => AddAt(index, value - this[index]);
        }

        /// <summary>
        /// BITの<c>index</c>番目の要素に<c>n</c>を加算します。
        /// </summary>
        /// <param name="index">加算するインデックス（0-indexed）</param>
        /// <param name="value">加算する数</param>
        public void AddAt(Index index, long value)
        {
            var i = index.GetOffset(Length);
            unchecked
            {
                if ((uint)i >= (uint)Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            i++;  // 1-indexedにする

            while (i <= Length)
            {
                _data[i] += value;
                i += i & -i;    // LSBの加算
            }
        }

        /// <summary>
        /// [0, <c>end</c>)の部分和を返します。
        /// </summary>
        /// <param name="end">部分和を求める半開区間の終了インデックス</param>
        /// <returns>区間の部分和</returns>
        public long Sum(Index end)
        {
            var i = end.GetOffset(Length);  // 0-indexedの半開区間＝1-indexedの閉区間なので+1は不要
            unchecked
            {
                if ((uint)i >= (uint)_data.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(end));
                }
            }

            long sum = 0;
            while (i > 0)
            {
                sum += _data[i];
                i -= i & -i;    // LSBの減算
            }
            return sum;
        }

        /// <summary>
        /// <c>range</c>の部分和を返します。
        /// </summary>
        /// <param name="range">部分和を求める半開区間</param>
        /// <returns>区間の部分和</returns>
        public long Sum(Range range) => Sum(range.End) - Sum(range.Start);

        /// <summary>
        /// [<c>start</c>, <c>end</c>)の部分和を返します。
        /// </summary>
        /// <param name="start">部分和を求める半開区間の開始インデックス</param>
        /// <param name="end">部分和を求める半開区間の終了インデックス</param>
        /// <returns>区間の部分和</returns>
        public long Sum(int start, int end) => Sum(end) - Sum(start);

        /// <summary>
        /// [0, <c>index</c>)の部分和が<c>sum</c>未満になる最大の<c>index</c>を返します。
        /// BIT上の各要素は0以上の数である必要があります。
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        public int GetLowerBound(long sum)
        {
            int index = 0;
            for (int offset = GetMostSignificantBitOf(Length); offset > 0; offset >>= 1)
            {
                if (index + offset < _data.Length && _data[index + offset] < sum)
                {
                    index += offset;
                    sum -= _data[index];
                }
            }

            return index;

            int GetMostSignificantBitOf(int n)
            {
                int k = 1;
                while ((k << 1) <= n)
                {
                    k <<= 1;
                };
                return k;
            }
        }
    }

    public class BinaryIndexedTree2D
    {
        long[,] _data;
        public int Height { get; }
        public int Width { get; }

        public BinaryIndexedTree2D(int height, int width)
        {
            Height = height;
            Width = width;
            _data = new long[height + 1, width + 1];
        }

        public long this[Index row, Index column]
        {
            get => Sum(row..(row.GetOffset(Height) + 1), column..(column.GetOffset(Width) + 1));
            set 
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)}は0以上の値である必要があります。");
                }
                AddAt(row, column, value - this[row, column]);
            }
        }

        /// <summary>
        /// 2次元BITの[<c>row</c>, <c>column</c>]に<c>value</c>を足します。
        /// </summary>
        /// <param name="row">加算する行（0-indexed）</param>
        /// <param name="column">加算する列（0-indexed）</param>
        /// <param name="value">加算する値</param>
        public void AddAt(Index row, Index column, long value)
        {
            var initI = row.GetOffset(Height);
            var initJ = column.GetOffset(Width);
            unchecked
            {
                if ((ulong)initI >= (ulong)Height)
                {
                    throw new ArgumentOutOfRangeException(nameof(row));
                }
                if ((ulong)initJ >= (ulong)Width)
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }
            }

            initI++;    // 1-indexed
            initJ++;    

            for (int i = initI; i <= Height; i += i & -i)
            {
                for (int j = initJ; j <= Width; j += j & -j)
                {
                    _data[i, j] += value;
                }
            }
        }

        /// <summary>
        /// 指定した半開区間の部分和を返します。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public long Sum(Index row, Index column)
        {
            long sum = 0;
            var initI = row.GetOffset(Height);
            var initJ = column.GetOffset(Width);
            unchecked
            {
                if ((ulong)initI >= (ulong)(Height + 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(row));
                }
                if ((ulong)initJ >= (ulong)(Width + 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }
            }

            for (int i = initI; i > 0; i -= i & -i)
            {
                for (int j = initJ; j > 0; j -= j & -j)
                {
                    sum += _data[i, j];
                }
            }
            return sum;
        }

        /// <summary>
        /// 指定した半開区間の部分和を返します。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public long Sum(Range rows, Range columns) => Sum(rows.End, columns.End) - Sum(rows.Start, columns.End) - Sum(rows.End, columns.Start) + Sum(rows.Start, columns.Start);

        /// <summary>
        /// 指定した半開区間の部分和を返します。
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="endRow"></param>
        /// <param name="beginColumn"></param>
        /// <param name="endColumn"></param>
        /// <returns></returns>
        public long Sum(int beginRow, int endRow, int beginColumn, int endColumn) => Sum(beginRow..endRow, beginColumn..endColumn);
    }

    /// <summary>
    /// <para>Red-Black tree which allows duplicated values (like multiset).</para>
    /// <para>Based on .NET Runtime https://github.com/dotnet/runtime/blob/master/src/libraries/System.Collections/src/System/Collections/Generic/SortedSet.cs </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    /// .NET Runtime
    ///   Copyright (c) .NET Foundation and Contributors
    ///   Released under the MIT license
    ///   https://github.com/dotnet/runtime/blob/master/LICENSE.TXT
    public class RedBlackTree<T> : ICollection<T>, IReadOnlyCollection<T> where T : IComparable<T>
    {
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        protected Node _root;

        public T this[Index index]
        {
            get
            {
                var i = index.GetOffset(Count);
                if (unchecked((uint)i) >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var current = _root;
                while (true)
                {
                    var leftCount = current.Left?.Count ?? 0;
                    if (leftCount == i)
                    {
                        return current.Item;
                    }
                    else if (leftCount > i)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        i -= leftCount + 1;
                        current = current.Right;
                    }
                }
            }
        }

        /// <summary>
        /// 最小の要素を返します。要素が空の場合、default値を返します。
        /// </summary>
        public T Min
        {
            get
            {
                if (_root == null)
                {
                    return default;
                }
                else
                {
                    var current = _root;
                    while (current.Left != null)
                    {
                        current = current.Left;
                    }
                    return current.Item;
                }
            }
        }

        /// <summary>
        /// 最大の要素を返します。要素が空の場合、default値を返します。
        /// </summary>
        public T Max
        {
            get
            {
                if (_root == null)
                {
                    return default;
                }
                else
                {
                    var current = _root;
                    while (current.Right != null)
                    {
                        current = current.Right;
                    }
                    return current.Item;
                }
            }
        }

        #region ICollection<T> members

        public void Add(T item)
        {
            if (_root == null)
            {
                _root = new Node(item, NodeColor.Black);
                Count = 1;
                return;
            }

            Node current = _root;
            Node parent = null;
            Node grandParent = null;        // 親、祖父はRotateで直接いじる
            Node greatGrandParent = null;   // 曾祖父はRotate後のつなぎ替えで使う（2回Rotateすると曾祖父がcurrentの親になる）

            var order = 0;
            while (current != null)
            {
                current.Count++;    // 部分木サイズ++
                order = item.CompareTo(current.Item);

                if (current.Is4Node)
                {
                    // 4-node (RBR) の場合は2-node x 2 (BRB) に変更
                    current.Split4Node();
                    if (Node.IsNonNullRed(parent))
                    {
                        // Splitの結果親と2連続でRRになったら修正
                        InsertionBalance(current, ref parent, grandParent, greatGrandParent);
                    }
                }

                greatGrandParent = grandParent;
                grandParent = parent;
                parent = current;
                current = order <= 0 ? current.Left : current.Right;
            }

            var newNode = new Node(item, NodeColor.Red);
            if (order <= 0)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }

            if (parent.IsRed)
            {
                // Redの親がRedのときは修正
                InsertionBalance(newNode, ref parent, grandParent, greatGrandParent);
            }

            _root.Color = NodeColor.Black;  // rootは常にBlack（Red->Blackとなったとき木全体の黒高さが1増える）
            Count++;
        }

        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            var current = _root;
            while (current != null)
            {
                var order = item.CompareTo(current.Item);
                if (order == 0)
                {
                    return true;
                }
                else
                {
                    current = order <= 0 ? current.Left : current.Right;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var value in this)
            {
                array[arrayIndex++] = value;
            }
        }

        public bool Remove(T item)
        {
            // .NET RuntimeのSortedSet<T>はややトリッキーな実装をしている。
            // 値の検索を行う際、検索パスにある全ての2-nodeを3-nodeまたは4-nodeに変更しつつ進んでいくのだが、
            // 各ノードに部分木のサイズを持たせたい場合、実装が難しくなるため、一般的な実装を用いることとする。
            // （削除が失敗した場合はサイズが変わらず、成功した場合のみサイズが変更となるため、パスを保存しておきたいのだが、
            // 　木を回転させながら検索を行うと木の親子関係が変化するため、パスも都度変更となってしまう。）

            var found = false;
            Node current = _root;
            var parents = new Stack<Node>(2 * Log2(Count + 1));  // 親ノードのスタック
            parents.Push(null); // 番兵

            while (current != null)
            {
                parents.Push(current);
                var order = item.CompareTo(current.Item);
                if (order == 0)
                {
                    found = true;
                    break;
                }
                else
                {
                    current = order < 0 ? current.Left : current.Right;
                }
            }

            if (!found)
            {
                // 見付からなければreturn
                return false;
            }

            // 子を2つ持つ場合
            if (current.Left != null && current.Right != null)
            {
                // 右部分木の最小ノードを探す
                parents.Push(current.Right);
                var minNode = GetMinNode(current.Right, parents);

                // この最小値の値だけもらってしまい、あとはこの最小値ノードを削除することを考えればよい
                current.Item = minNode.Item;
                current = minNode;
            }

            // 通ったパス上にある部分木のサイズを全て1だけデクリメント
            parents.Pop();  // これは今から消すので不要
            Count--;
            foreach (var node in parents)
            {
                if (node != null)
                {
                    node.Count--;
                }
            }

            // 切り離した部分をくっつける。子を2つ持つ場合については上で考えたため、子を0or1つ持つ場合を考えればよい
            // 二分木の黒高さが全て等しいという条件より、片方だけ2個以上伸びているということは起こり得ない
            var parent = parents.Peek();
            ReplaceChildOrRoot(parent, current, current.Left ?? current.Right);  // L/Rのどちらかnullでない方。どちらもnullならnullが入る

            // 削除するノードが赤の場合は修正不要
            if (current.IsRed)
            {
                return true;
            }

            // つなぎ替えられた方の子
            current = current.Left ?? current.Right;

            while ((parent = parents.Pop()) != null)
            {
                var toFix = DeleteBalance(current, parent, out var newParent);
                ReplaceChildOrRoot(parents.Peek(), parent, newParent);

                if (!toFix)
                {
                    break;
                }
                current = newParent;
            }

            if (_root != null)
            {
                _root.Color = NodeColor.Black;
            }
            return true;
        }

        private static Node GetMinNode(Node current, Stack<Node> parents)
        {
            while (current.Left != null)
            {
                current = current.Left;
                parents.Push(current);
            }
            return current;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_root != null)
            {
                var stack = new Stack<Node>(2 * Log2(Count + 1));
                PushLeft(stack, _root);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    yield return current.Item;
                    PushLeft(stack, current.Right);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary>
        /// <paramref name="item"/>未満の最大の要素を取得します。見付からなかった場合、defaultを返します。
        /// </summary>
        public T GetLessThan(T item)
        {
            var current = _root;
            var result = default(T);

            while (current != null)
            {
                var order = current.Item.CompareTo(item);
                if (order < 0)
                {
                    result = current.Item;
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }
            }

            return result;
        }

        /// <summary>
        /// <paramref name="item"/>以上の最小の要素を取得します。見付からなかった場合、defaultを返します。
        /// </summary>
        public T GetGreaterEqual(T item)
        {
            var current = _root;
            var result = default(T);

            while (current != null)
            {
                var order = current.Item.CompareTo(item);
                if (order >= 0)
                {
                    result = current.Item;
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            return result;
        }

        /// <summary>
        /// [<paramref name="inclusiveMin"/>, <paramref name="exclusiveMax"/>)を満たす要素を昇順に列挙します。
        /// </summary>
        /// <param name="inclusiveMin">区間の最小値（それ自身を含む）</param>
        /// <param name="exclusiveMax">区間の最大値（それ自身を含まない）</param>
        /// <returns></returns>
        public IEnumerable<T> EnumerateRange(T inclusiveMin, T exclusiveMax)
        {
            if (_root != null)
            {
                var stack = new Stack<Node>(2 * Log2(Count + 1));
                var current = _root;
                while (current != null)
                {
                    var order = current.Item.CompareTo(inclusiveMin);
                    if (order >= 0)
                    {
                        stack.Push(current);
                        current = current.Left;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }

                while (stack.Count > 0)
                {
                    current = stack.Pop();
                    var order = current.Item.CompareTo(exclusiveMax);
                    if (order >= 0)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return current.Item;
                        PushLeft(stack, current.Right);
                    }
                }
            }
        }

        private static void PushLeft(Stack<Node> stack, Node node)
        {
            while (node != null)
            {
                stack.Push(node);
                node = node.Left;
            }
        }

        private static int Log2(int n)
        {
            int result = 0;
            while (n > 0)
            {
                result++;
                n >>= 1;
            }
            return result;
        }

        // After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
        // It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
        // need to split again in the next node.
        // By the time we need to split again, everything will be correctly set.
        private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(grandParent != null);

            var parentIsOnRight = grandParent.Right == parent;
            var currentIsOnRight = parent.Right == current;

            Node newChildOfGreatGrandParent;
            if (parentIsOnRight == currentIsOnRight)
            {
                // LL or RRなら1回転でOK
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft() : grandParent.RotateRight();
            }
            else
            {
                // LR or RLなら2回転
                newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft();
                // 1回転ごとに1つ上に行くため、2回転させると曾祖父が親になる
                // リンク先「挿入操作」を参照 http://wwwa.pikara.ne.jp/okojisan/rb-tree/index.html
                parent = greatGrandParent;
            }

            // 祖父は親の子（1回転）もしくは自分の子（2回転）のいずれかになる
            // この時点で色がRRBもしくはBRRになっているため、BRBに修正
            // リンク先「挿入操作」を参照 http://wwwa.pikara.ne.jp/okojisan/rb-tree/index.html
            grandParent.Color = NodeColor.Red;
            newChildOfGreatGrandParent.Color = NodeColor.Black;

            ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
        }

        private bool DeleteBalance(Node current, Node parent, out Node newParent)
        {
            // 削除パターンは大きく分けて4つ
            // See: http://wwwa.pikara.ne.jp/okojisan/rb-tree/index.html

            // currentはもともと黒なので（黒でなければ修正する必要がないため）、兄弟はnullにはなり得ない
            var sibling = parent.GetSibling(current);
            if (sibling.IsBlack)
            {
                if (Node.IsNonNullRed(sibling.Left) || Node.IsNonNullRed(sibling.Right))
                {
                    var parentColor = parent.Color;
                    var siblingRedChild = Node.IsNonNullRed(sibling.Left) ? sibling.Left : sibling.Right;
                    var currentIsOnRight = parent.Right == current;
                    var siblingRedChildIsRight = sibling.Right == siblingRedChild;

                    if (currentIsOnRight != siblingRedChildIsRight)
                    {
                        // 1回転
                        parent.Color = NodeColor.Black;
                        sibling.Color = parentColor;
                        siblingRedChild.Color = NodeColor.Black;
                        newParent = currentIsOnRight ? parent.RotateRight() : parent.RotateLeft();
                    }
                    else
                    {
                        // 2回転
                        parent.Color = NodeColor.Black;
                        siblingRedChild.Color = parentColor;
                        newParent = currentIsOnRight ? parent.RotateLeftRight() : parent.RotateRightLeft();
                    }

                    return false;
                }
                else
                {
                    var needToFix = parent.IsBlack;
                    parent.Color = NodeColor.Black;
                    sibling.Color = NodeColor.Red;
                    newParent = parent;
                    return needToFix;
                }
            }
            else
            {
                if (current == parent.Right)
                {
                    newParent = parent.RotateRight();
                }
                else
                {
                    newParent = parent.RotateLeft();
                }

                parent.Color = NodeColor.Red;
                sibling.Color = NodeColor.Black;
                DeleteBalance(current, parent, out var newChildOfParent);  // 再帰は1回限り
                ReplaceChildOrRoot(newParent, parent, newChildOfParent);
                return false;
            }
        }

        /// <summary>
        /// 親ノードの子を新しいものに差し替える。ただし親がいなければrootとする。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="newChild"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReplaceChildOrRoot(Node parent, Node child, Node newChild)
        {
            if (parent != null)
            {
                parent.ReplaceChild(child, newChild);
            }
            else
            {
                _root = newChild;
            }
        }

        #region Debugging

        [Conditional("DEBUG")]
        internal void PrintTree() => PrintTree(_root, 0);

        [Conditional("DEBUG")]
        private void PrintTree(Node node, int depth)
        {
            const int Space = 6;
            if (node != null)
            {
                PrintTree(node.Right, depth + 1);
                if (node.IsRed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(string.Concat(Enumerable.Repeat(' ', Space * depth)));
                    Console.WriteLine($"{node.Item} ({node.Count})");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(string.Concat(Enumerable.Repeat(' ', Space * depth)));
                    Console.WriteLine($"{node.Item} ({node.Count})");
                }
                PrintTree(node.Left, depth + 1);
            }
        }

        [Conditional("DEBUG")]
        internal void AssertCorrectRedBrackTree() => AssertCorrectRedBrackTree(_root);

        private int AssertCorrectRedBrackTree(Node node)
        {
            if (node != null)
            {
                // Redが2つ繋がっていないか？
                Debug.Assert(!(node.IsRed && Node.IsNonNullRed(node.Left)));
                Debug.Assert(!(node.IsRed && Node.IsNonNullRed(node.Right)));

                // 左右の黒高さは等しいか？
                var left = AssertCorrectRedBrackTree(node.Left);
                var right = AssertCorrectRedBrackTree(node.Right);
                Debug.Assert(left == right);
                if (node.IsBlack)
                {
                    left++;
                }
                return left;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        protected enum NodeColor : byte
        {
            Black,
            Red
        }

        [DebuggerDisplay("Item = {Item}, Size = {Count}")]
        protected sealed class Node
        {
            public T Item { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public NodeColor Color { get; set; }
            /// <summary>部分木のサイズ</summary>
            public int Count { get; set; }

            public bool IsBlack => Color == NodeColor.Black;
            public bool IsRed => Color == NodeColor.Red;
            public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);
            public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdateCount() => Count = GetCountOrDefault(Left) + GetCountOrDefault(Right) + 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsNonNullBlack(Node node) => node != null && node.IsBlack;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsNonNullRed(Node node) => node != null && node.IsRed;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsNullOrBlack(Node node) => node == null || node.IsBlack;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int GetCountOrDefault(Node node) => node?.Count ?? 0;    // C# 6.0 or later

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node(T item, NodeColor color)
            {
                Item = item;
                Color = color;
                Count = 1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Split4Node()
            {
                Color = NodeColor.Red;
                Left.Color = NodeColor.Black;
                Right.Color = NodeColor.Black;
            }

            // 各種Rotateでは位置関係だけ修正する。色までは修正しない。
            // 親になったノード（部分木の根）を返り値とする。
            // childとかgrandChildとかは祖父（Rotate前の3世代中一番上）目線での呼び方
            public Node RotateLeft()
            {
                // 右の子が自分の親になる
                var child = Right;
                Right = child.Left;
                child.Left = this;
                UpdateCount();
                child.UpdateCount();
                return child;
            }

            public Node RotateRight()
            {
                // 左の子が自分の親になる
                var child = Left;
                Left = child.Right;
                child.Right = this;
                UpdateCount();
                child.UpdateCount();
                return child;
            }

            public Node RotateLeftRight()
            {
                var child = Left;
                var grandChild = child.Right;

                Left = grandChild.Right;
                grandChild.Right = this;
                child.Right = grandChild.Left;
                grandChild.Left = child;
                UpdateCount();
                child.UpdateCount();
                grandChild.UpdateCount();
                return grandChild;
            }

            public Node RotateRightLeft()
            {
                var child = Right;
                var grandChild = child.Left;

                Right = grandChild.Left;
                grandChild.Left = this;
                child.Left = grandChild.Right;
                grandChild.Right = child;
                UpdateCount();
                child.UpdateCount();
                grandChild.UpdateCount();
                return grandChild;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ReplaceChild(Node child, Node newChild)
            {
                if (Left == child)
                {
                    Left = newChild;
                }
                else
                {
                    Right = newChild;
                }
            }

            /// <summary>
            /// 兄弟を取得する。
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node GetSibling(Node node) => node == Left ? Right : Left;

            /// <summary>
            /// 左右の2-nodeを4-nodeにマージする。
            /// </summary>
            public void Merge2Nodes()
            {
                Color = NodeColor.Black;
                Left.Color = NodeColor.Red;
                Right.Color = NodeColor.Red;
            }
        }
    }

    public class Trie
    {
        private Stack<Node> _nodeStack = new Stack<Node>();

        /// <summary>
        /// 最大の文字サイズ。英小文字なら26。
        /// </summary>
        public int MaxChar { get; }
        public char StartChar { get; }
        public char EndChar { get; }
        public int Count => Root.Count;

        public Node Root { get; private set; }

        /// <summary>
        /// <see cref="Trie"/>クラスのインスタンスを作成します。
        /// <example>
        /// <code>
        /// var trie = new <see cref="Trie"/>('a', 'z');
        /// </code>
        /// </example>
        /// </summary>
        public Trie(char inclusiveStartChar, char inclusiveEndChar)
        {
            if (inclusiveStartChar > inclusiveEndChar)
            {
                throw new ArgumentException();
            }

            StartChar = inclusiveStartChar;
            EndChar = inclusiveEndChar;
            MaxChar = inclusiveEndChar - inclusiveStartChar + 1;
            Root = new Node(MaxChar);
        }

        public bool Add(string s) => Add(s.AsSpan());

        public bool Add(ReadOnlySpan<char> s)
        {
            var current = Root;
            _nodeStack.Clear();
            _nodeStack.Push(current);

            foreach (var c in s)
            {
                var index = c - StartChar;
                current = current._children[index] ??= new Node(MaxChar);
                _nodeStack.Push(current);
            }

            if (current.Acceptable)
            {
                return false;
            }
            else
            {
                current.Acceptable = true;
                while (_nodeStack.Count > 0)
                {
                    _nodeStack.Pop().Count++;
                }
                return true;
            }
        }

        /// <summary>
        /// 指定した単語<paramref name="s"/>が含まれるかどうかを調べます。
        /// </summary>
        /// <param name="s">検索文字列</param>
        /// <param name="isPrefix">検索文字列自体が登録されていなくても、登録単語のprefixであればよいならばtrue, そうでないならfalse。</param>
        public bool Contains(string s, bool isPrefix = false) => Contains(s.AsSpan(), isPrefix);

        /// <summary>
        /// 指定した単語<paramref name="s"/>が含まれるかどうかを調べます。
        /// </summary>
        /// <param name="s">検索文字列</param>
        /// <param name="isPrefix">検索文字列自体が登録されていなくても、登録単語のprefixであればよいならばtrue, そうでないならfalse。</param>
        public bool Contains(ReadOnlySpan<char> s, bool isPrefix = false)
        {
            var current = Root;
            foreach (var c in s)
            {
                var index = c - StartChar;

                if (current._children[index] == null)
                {
                    return false;
                }
                else
                {
                    current = current._children[index];
                }
            }

            return current.Acceptable || isPrefix;
        }

        /// <summary>
        /// <paramref name="s"/>の空でないprefixのうち、登録されているものの一覧を取得します。
        /// </summary>
        /// <param name="s">検索文字列</param>
        public List<ReadOnlyMemory<char>> GetAllPrefix(string s) => GetAllPrefix(s.AsMemory());

        /// <summary>
        /// <paramref name="s"/>の空でないprefixのうち、登録されているものの一覧を取得します。
        /// </summary>
        /// <param name="s">検索文字列</param>
        public List<ReadOnlyMemory<char>> GetAllPrefix(ReadOnlyMemory<char> s)
        {
            var current = Root;
            var result = new List<ReadOnlyMemory<char>>();
            var i = 0;

            foreach (var c in s.Span)
            {
                i++;
                var index = c - StartChar;

                if (current._children[index] == null)
                {
                    return result;
                }
                else
                {
                    current = current._children[index];

                    if (current.Acceptable)
                    {
                        result.Add(s.Slice(0, i));
                    }
                }
            }

            return result;
        }

        public void Clear() => Root = new Node(MaxChar);

        public class Node
        {
            // 親クラス以外には見えないようにしたいけど厳しい……
            internal readonly Node[] _children;
            public ReadOnlySpan<Node> Children => _children;
            public int Count { get; internal set; }
            public bool Acceptable { get; internal set; }

            public Node(int maxChar)
            {
                _children = new Node[maxChar];
            }
        }
    }

    public class Counter<T> : IEnumerable<(T key, long count)> where T : IEquatable<T>
    {
        private Dictionary<T, long> _innerDictionary;

        public Counter()
        {
            _innerDictionary = new Dictionary<T, long>();
        }

        public IEnumerator<(T key, long count)> GetEnumerator()
        {
            foreach (var pair in _innerDictionary)
            {
                yield return (key: pair.Key, count: pair.Value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public long this[T key]
        {
            get
            {
                _innerDictionary.TryGetValue(key, out var count);
                return count;
            }
            set
            {
                _innerDictionary[key] = value;
            }
        }
    }

    public readonly struct BitSet : IEquatable<BitSet>
    {
        readonly uint _value;

        public BitSet(uint value)
        {
            _value = value;
        }

        public bool this[int digit]
        {
            get => ((_value >> digit) & 1) > 0;
        }

        public bool Any => _value > 0;
        public bool None => _value == 0;
        public BitSet SetAt(int digit, bool value) => value ? new BitSet(_value | (1u << digit)) : new BitSet(_value & ~(1u << digit));
        public BitSet Lsb() { unchecked { return new BitSet(_value & (uint)-(int)_value); } }

        public BitSet Reverse()
        {
            unchecked
            {
                uint v = _value;
                v = (v & 0x55555555) << 1 | (v >> 1 & 0x55555555);
                v = (v & 0x33333333) << 2 | (v >> 2 & 0x33333333);
                v = (v & 0x0f0f0f0f) << 4 | (v >> 4 & 0x0f0f0f0f);
                v = (v & 0x00ff00ff) << 8 | (v >> 8 & 0x00ff00ff);
                v = (v & 0x0000ffff) << 16 | (v >> 16 & 0x0000ffff);
                return new BitSet(v);
            }
        }

        public int PopCount() => BitOperations.PopCount(_value);

        public static BitSet Zero => new BitSet(0);
        public static BitSet One => new BitSet(1);
        public static BitSet All => new BitSet(~0u);
        public static BitSet At(int digit) => new BitSet(1u << digit);
        public static BitSet CreateMask(int digit) => new BitSet((1u << digit) - 1);
        public static BitSet operator ++(BitSet bitSet) => new BitSet(bitSet._value + 1);
        public static BitSet operator --(BitSet bitSet) => new BitSet(bitSet._value - 1);
        public static BitSet operator ~(BitSet bitSet) => new BitSet(~bitSet._value);
        public static BitSet operator &(BitSet left, BitSet right) => new BitSet(left._value & right._value);
        public static BitSet operator |(BitSet left, BitSet right) => new BitSet(left._value | right._value);
        public static BitSet operator ^(BitSet left, BitSet right) => new BitSet(left._value ^ right._value);
        public static BitSet operator <<(BitSet bitSet, int n) => new BitSet(bitSet._value << n);
        public static BitSet operator >>(BitSet bitSet, int n) => new BitSet(bitSet._value >> n);
        public static bool operator <(BitSet left, BitSet right) => left._value < right._value;
        public static bool operator <=(BitSet left, BitSet right) => left._value <= right._value;
        public static bool operator >(BitSet left, BitSet right) => left._value > right._value;
        public static bool operator >=(BitSet left, BitSet right) => left._value >= right._value;
        public static bool operator ==(BitSet left, BitSet right) => left.Equals(right);
        public static bool operator !=(BitSet left, BitSet right) => !(left == right);
        public static implicit operator uint(BitSet bitSet) => bitSet._value;

        public override bool Equals(object obj) => obj is BitSet set && Equals(set);
        public bool Equals(BitSet other) => _value == other._value;
        public override string ToString() => Convert.ToString(_value, 2);
        public override int GetHashCode() => _value.GetHashCode();
    }

    public static class PermutationAlgorithms
    {
        public static IEnumerable<ReadOnlyMemory<T>> GetPermutations<T>(IEnumerable<T> collection) where T : IComparable<T> => GetPermutations(collection, false);

        public static IEnumerable<ReadOnlyMemory<T>> GetPermutations<T>(IEnumerable<T> collection, bool isSorted) where T : IComparable<T>
        {
            var a = collection.ToArray();

            if (!isSorted && a.Length > 1)
            {
                Array.Sort(a);
            }

            yield return a; // ソート済み初期配列

            if (a.Length <= 2)
            {
                if (a.Length == 2 && a[0].CompareTo(a[1]) != 0)
                {
                    (a[0], a[1]) = (a[1], a[0]);
                    yield return a;
                    yield break;
                }

                yield break;
            }

            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = a.Length - 2; i >= 0; i--)
                {
                    // iよりi+1の方が大きい（昇順）なら
                    if (a[i].CompareTo(a[i + 1]) < 0)
                    {
                        // 後ろから見ていってi<jとなるところを探して
                        int j;
                        for (j = a.Length - 1; a[i].CompareTo(a[j]) >= 0; j--) { }

                        // iとjを入れ替えて
                        (a[i], a[j]) = (a[j], a[i]);

                        // i+1以降を反転
                        if (i < a.Length - 2)
                        {
                            var sliced = a.AsSpan().Slice(i + 1);
                            sliced.Reverse();
                        }

                        flag = true;
                        yield return a;
                        break;
                    }
                }
            }
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<(T1 v1, T2 v2)> Zip<T1, T2>(this (IEnumerable<T1> First, IEnumerable<T2> Second) t) 
            => t.First.Zip(t.Second, (v1, v2) => (v1, v2));

        public static IEnumerable<(T1 v1, T2 v2, T3 v3)> Zip<T1, T2, T3>(this (IEnumerable<T1> First, IEnumerable<T2> Second, IEnumerable<T3> Third) t) 
            => (t.First, t.Second).Zip().Zip(t.Third, (v12, v3) => (v12.v1, v12.v2, v3));

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) => source.Select((item, index) => (item, index));
    }
}

#endregion

#region Graphs

namespace AtCoderTemplateForNetCore.Graphs
{
    public interface IEdge
    {
        int To { get; }
    }

    public interface IWeightedEdge : IEdge
    {
        long Weight { get; }
    }

    public interface IGraph<TEdge> where TEdge : IEdge
    {
        ReadOnlySpan<TEdge> this[int node] { get; }
        int NodeCount { get; }
    }

    public interface IWeightedGraph<TEdge> : IGraph<TEdge> where TEdge : IWeightedEdge {  }

    public readonly struct BasicEdge : IEdge
    {
        public int To { get; }

        public BasicEdge(int to)
        {
            To = to;
        }

        public override string ToString() => To.ToString();
        public static implicit operator BasicEdge(int edge) => new BasicEdge(edge);
        public static implicit operator int(BasicEdge edge) => edge.To;
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct WeightedEdge : IWeightedEdge
    {
        public int To { get; }
        public long Weight { get; }

        public WeightedEdge(int to) : this(to, 1) { }

        public WeightedEdge(int to, long weight)
        {
            To = to;
            Weight = weight;
        }

        public override string ToString() => $"[{Weight}]-->{To}";
        public void Deconstruct(out int to, out long weight) => (to, weight) = (To, Weight);
    }

    public class BasicGraph : IGraph<BasicEdge>
    {
        private readonly List<List<BasicEdge>> _edges;
        public ReadOnlySpan<BasicEdge> this[int node] => _edges[node].AsSpan();
        public int NodeCount => _edges.Count;

        public BasicGraph(int nodeCount)
        {
            _edges = new List<List<BasicEdge>>(nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                _edges.Add(new List<BasicEdge>());
            }
        }

        public void AddEdge(int from, int to) => _edges[from].Add(to);
        public void AddNode() => _edges.Add(new List<BasicEdge>());
    }

    public class WeightedGraph : IGraph<WeightedEdge>
    {
        private readonly List<List<WeightedEdge>> _edges;
        public ReadOnlySpan<WeightedEdge> this[int node] => _edges[node].AsSpan();
        public int NodeCount => _edges.Count;

        public WeightedGraph(int nodeCount)
        {
            _edges = new List<List<WeightedEdge>>(nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                _edges.Add(new List<WeightedEdge>());
            }
        }

        public void AddEdge(int from, int to, long weight) => _edges[from].Add(new WeightedEdge(to, weight));
        public void AddNode() => _edges.Add(new List<WeightedEdge>());
    }

    namespace Algorithms
    {
        public class Dijkstra
        {
            private readonly WeightedGraph _graph;

            public Dijkstra(WeightedGraph graph)
            {
                _graph = graph;
            }

            public long[] GetDistancesFrom(int startNode)
            {
                const long Inf = 1L << 60;
                var distances = Enumerable.Repeat(Inf, _graph.NodeCount).ToArray();
                distances[startNode] = 0;
                var todo = new PriorityQueue<State>(PriorityQueue<State>.Order.Ascending);
                todo.Enqueue(new State(startNode, 0));

                while (todo.Count > 0)
                {
                    var current = todo.Dequeue();
                    if (current.Distance > distances[current.Node])
                    {
                        continue;
                    }

                    foreach (var edge in _graph[current.Node])
                    {
                        var nextDistance = current.Distance + edge.Weight;
                        if (distances[edge.To] > nextDistance)
                        {
                            distances[edge.To] = nextDistance;
                            todo.Enqueue(new State(edge.To, nextDistance));
                        }
                    }
                }

                return distances;
            }

            private readonly struct State : IComparable<State>
            {
                public int Node { get; }
                public long Distance { get; }

                public State(int node, long distance)
                {
                    Node = node;
                    Distance = distance;
                }

                public int CompareTo(State other) => Distance.CompareTo(other.Distance);
                public void Deconstruct(out int node, out long distance) => (node, distance) = (Node, Distance);
            }
        }

        public class BellmanFord
        {
            private readonly List<Edge> _edges = new List<Edge>();
            protected readonly int _nodeCount;

            public BellmanFord(int nodeCount)
            {
                _nodeCount = nodeCount;
            }

            public BellmanFord(WeightedGraph graph)
            {
                _nodeCount = graph.NodeCount;

                for (int from = 0; from < graph.NodeCount; from++)
                {
                    foreach (var edge in graph[from])
                    {
                        AddEdge(from, edge.To, edge.Weight);
                    }
                }
            }


            public void AddEdge(int from, int to, long weight) => _edges.Add(new Edge(from, to, weight));

            public (long[] distances, bool[] isNegativeCycle) GetDistancesFrom(int startNode)
            {
                const long Inf = long.MaxValue >> 1;
                var distances = new long[_nodeCount];
                distances.AsSpan().Fill(Inf);
                var isNegativeCycle = new bool[_nodeCount];
                distances[startNode] = 0;

                for (int i = 1; i <= 2 * _nodeCount; i++)
                {
                    foreach (var edge in _edges.AsSpan())
                    {
                        // そもそも出発点に未到達なら無視
                        if (distances[edge.From] < Inf)
                        {
                            if (i <= _nodeCount)
                            {
                                var newCost = distances[edge.From] + edge.Weight;
                                if (distances[edge.To] > newCost)
                                {
                                    distances[edge.To] = newCost;
                                    // N回目に更新されたやつにチェックを付けて、追加でN回伝播させる
                                    if (i == _nodeCount)
                                    {
                                        isNegativeCycle[edge.To] = true;
                                    }
                                }
                            }
                            else if (isNegativeCycle[edge.From])
                            {
                                isNegativeCycle[edge.To] = true;
                            }
                        }
                    }
                }

                // 一応キレイにしておく
                for (int i = 0; i < _nodeCount; i++)
                {
                    if (isNegativeCycle[i])
                    {
                        distances[i] = long.MinValue;
                    }
                    else if (distances[i] >= Inf)
                    {
                        distances[i] = long.MaxValue;
                    }
                }

                return (distances, isNegativeCycle);
            }

            [StructLayout(LayoutKind.Auto)]
            readonly struct Edge
            {
                public int From { get; }
                public int To { get; }
                public long Weight { get; }

                public Edge(int from, int to, long cost)
                {
                    From = from;
                    To = to;
                    Weight = cost;
                }

                public void Deconstruct(out int from, out int to, out long cost) => (from, to, cost) = (From, To, Weight);
                public override string ToString() => $"{nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Weight)}: {Weight}";
            }
        }

        public class MaxFlow
        {
            private readonly List<InternalEdge>[] _graph;
            private readonly List<(int v, int index)> _edgeIndice;
            public int VertexCount => _graph.Length;

            public MaxFlow(int n)
            {
                _graph = Enumerable.Repeat(0, n).Select(_ => new List<InternalEdge>()).ToArray();
                _edgeIndice = new List<(int from, int to)>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddEdge(int from, int to, int capacity)
            {
                _edgeIndice.Add((from, _graph[from].Count));
                _graph[from].Add(new InternalEdge(to, _graph[to].Count, capacity));
                _graph[to].Add(new InternalEdge(from, _graph[from].Count - 1, 0));
            }

            public IEnumerable<Edge> GetEdges()
            {
                for (int i = 0; i < _edgeIndice.Count; i++)
                {
                    var (v, index) = _edgeIndice[i];
                    var edge = _graph[v][index];
                    var invEdge = _graph[edge.To][edge.InvIndex];
                    yield return new Edge(v, edge.To, edge.Capacity + invEdge.Capacity, invEdge.Capacity);
                }
            }

            public int Flow(int source, int sink) => Flow(source, sink, int.MaxValue);

            public int Flow(int source, int sink, int flowLimit)
            {
                var distances = new int[VertexCount];
                var iterations = new int[VertexCount];
                var queue = new Queue<int>();

                var flow = 0;

                while (flow < flowLimit)
                {
                    Bfs();
                    if (distances[sink] == -1)
                    {
                        break;
                    }

                    iterations.AsSpan().Clear();
                    while (flow < flowLimit)
                    {
                        var dFlow = Dfs(sink, flowLimit - flow);
                        if (dFlow == 0)
                        {
                            break;
                        }
                        flow += dFlow;
                    }
                }

                return flow;

                void Bfs()
                {
                    distances.Fill(-1);
                    distances[source] = 0;
                    queue.Clear();
                    queue.Enqueue(source);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        var distance = distances[current];
                        foreach (ref readonly var edge in _graph[current].AsSpan())
                        {
                            if (edge.Capacity == 0 || distances[edge.To] >= 0)
                            {
                                continue;
                            }

                            distances[edge.To] = distance + 1;

                            if (edge.To == sink)
                            {
                                return;
                            }

                            queue.Enqueue(edge.To);
                        }
                    }
                }

                int Dfs(int v, int flowLimit)
                {
                    if (v == source)
                    {
                        return flowLimit;
                    }

                    var result = 0;
                    var edges = _graph[v].AsSpan();
                    var distance = distances[v];

                    for (ref var iteration = ref iterations[v]; iteration < edges.Length; iteration++)
                    {
                        ref var edge = ref edges[iteration];
                        ref var invEdge = ref _graph[edge.To].AsSpan()[edge.InvIndex];
                        if (distance <= distances[edge.To] || invEdge.Capacity == 0)
                        {
                            continue;
                        }

                        var deltaFlow = Dfs(edge.To, Math.Min(flowLimit - result, invEdge.Capacity));

                        if (deltaFlow <= 0)
                        {
                            continue;
                        }

                        edge += deltaFlow;
                        invEdge -= deltaFlow;
                        result += deltaFlow;

                        if (result == flowLimit)
                        {
                            break;
                        }
                    }

                    return result;
                }
            }

            public bool[] GetMinCut(int source)
            {
                var visited = new bool[VertexCount];
                var queue = new Queue<int>();
                queue.Enqueue(source);

                while (queue.Count > 0)
                {
                    var v = queue.Dequeue();
                    visited[v] = true;
                    foreach (var edge in _graph[v].AsSpan())
                    {
                        if (edge.Capacity > 0 && !visited[edge.To])
                        {
                            visited[edge.To] = true;
                            queue.Enqueue(edge.To);
                        }
                    }
                }

                return visited;
            }

            [StructLayout(LayoutKind.Auto)]
            public readonly struct Edge
            {
                public int From { get; }
                public int To { get; }
                public int Capacity { get; }
                public int Flow { get; }

                public Edge(int from, int to, int capacity, int flow)
                {
                    From = from;
                    To = to;
                    Capacity = capacity;
                    Flow = flow;
                }

                public void Deconstruct(out int to, out int inv, out int capacity, out int flow)
                    => (to, inv, capacity, flow) = (From, To, Capacity, Flow);
                public override string ToString() => $"{nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Capacity)}: {Capacity}, {nameof(Flow)}: {Flow}";
            }

            [StructLayout(LayoutKind.Auto)]
            private readonly struct InternalEdge
            {
                public int To { get; }
                public int InvIndex { get; }
                public int Capacity { get; }

                public InternalEdge(int to, int invIndex, int capacity)
                {
                    To = to;
                    InvIndex = invIndex;
                    Capacity = capacity;
                }

                public static InternalEdge operator +(InternalEdge edge, int flow) => new InternalEdge(edge.To, edge.InvIndex, edge.Capacity + flow);
                public static InternalEdge operator -(InternalEdge edge, int flow) => new InternalEdge(edge.To, edge.InvIndex, edge.Capacity - flow);
                public override string ToString() => $"{nameof(To)}: {To}, {nameof(InvIndex)}: {InvIndex}, {nameof(Capacity)}: {Capacity}";
            }
        }

        public class MinCostFlow
        {
            private readonly List<InternalEdge>[] _graph;
            private readonly List<(int v, int index)> _edgeIndice;
            public int VertexCount => _graph.Length;

            public MinCostFlow(int n)
            {
                _graph = Enumerable.Repeat(0, n).Select(_ => new List<InternalEdge>()).ToArray();
                _edgeIndice = new List<(int v, int index)>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddEdge(int from, int to, int capacity, long cost)
            {
                _edgeIndice.Add((from, _graph[from].Count));
                _graph[from].Add(new InternalEdge(to, _graph[to].Count, capacity, cost));
                _graph[to].Add(new InternalEdge(from, _graph[from].Count - 1, 0, -cost));
            }

            public IEnumerable<Edge> GetEdges()
            {
                for (int i = 0; i < _edgeIndice.Count; i++)
                {
                    var (v, index) = _edgeIndice[i];
                    var edge = _graph[v][index];
                    var invEdge = _graph[edge.To][edge.InvIndex];
                    yield return new Edge(v, edge.To, edge.Capacity + invEdge.Capacity, invEdge.Capacity, edge.Cost);
                }
            }

            public (int flow, long cost) Flow(int source, int sink) => Flow(source, sink, int.MaxValue);

            public (int flow, long cost) Flow(int source, int sink, int flowLimit) => GetCostSlope(source, sink, flowLimit)[^1];

            public ReadOnlySpan<(int flow, long cost)> GetCostSlope(int source, int sink) => GetCostSlope(source, sink, int.MaxValue);

            public ReadOnlySpan<(int flow, long cost)> GetCostSlope(int source, int sink, int flowLimit)
            {
                if (source == sink)
                {
                    throw new ArgumentException();
                }

                var potentials = new long[VertexCount];
                var distances = new long[VertexCount];
                var prevVertice = new int[VertexCount];
                var prevEdges = new int[VertexCount];
                var visited = new bool[VertexCount];

                var flow = 0;
                long cost = 0;
                long prevCost = -1;
                var result = new List<(int flow, long cost)>();
                result.Add((flow, cost));

                while (flow < flowLimit)
                {
                    if (!Update())
                    {
                        break;
                    }

                    var capacity = flowLimit - flow;

                    var v = sink;
                    while (v != source)
                    {
                        var pv = prevVertice[v];
                        var pe = prevEdges[v];
                        capacity.ChangeMin(_graph[pv][pe].Capacity);
                        v = pv;
                    }

                    v = sink;
                    while (v != source)
                    {
                        var pv = prevVertice[v];
                        var pe = prevEdges[v];
                        ref var edge = ref _graph[pv].AsSpan()[pe];
                        ref var invEdge = ref _graph[v].AsSpan()[edge.InvIndex];

                        edge = edge.Flow(-capacity);
                        invEdge = invEdge.Flow(capacity);

                        v = pv;
                    }

                    var d = -potentials[source];
                    flow += capacity;
                    cost += capacity * d;
                    if (prevCost == d)
                    {
                        result.RemoveAt(result.Count - 1);
                    }
                    result.Add((flow, cost));
                    prevCost = cost;
                }

                return result.AsSpan();

                bool Update()
                {
                    distances.AsSpan().Fill(long.MaxValue);
                    prevVertice.AsSpan().Fill(-1);
                    prevEdges.AsSpan().Fill(-1);
                    visited.AsSpan().Clear();

                    var queue = new PriorityQueue<DijkstraState>(PriorityQueue<DijkstraState>.Order.Ascending);
                    distances[source] = 0;
                    queue.Enqueue(new DijkstraState(source, 0));

                    while (queue.Count > 0)
                    {
                        var (current, currentDistance) = queue.Dequeue();
                        if (visited[current])
                        {
                            continue;
                        }

                        visited[current] = true;
                        if (current == sink)
                        {
                            break;
                        }

                        var edges = _graph[current].AsSpan();

                        for (int i = 0; i < edges.Length; i++)
                        {
                            ref readonly var edge = ref edges[i];
                            if (visited[edge.To] || edge.Capacity == default)
                            {
                                continue;
                            }

                            var cost = edge.Cost - potentials[edge.To] + potentials[current];
                            ref var nextDistance = ref distances[edge.To];
                            if (nextDistance - currentDistance > cost)
                            {
                                nextDistance = currentDistance + cost;
                                prevVertice[edge.To] = current;
                                prevEdges[edge.To] = i;
                                queue.Enqueue(new DijkstraState(edge.To, nextDistance));
                            }
                        }
                    }

                    if (!visited[sink])
                    {
                        return false;
                    }

                    var sinkDistance = distances[sink];
                    for (int v = 0; v < visited.Length; v++)
                    {
                        if (visited[v])
                        {
                            potentials[v] -= sinkDistance - distances[v];
                        }
                    }

                    return true;
                }
            }

            [StructLayout(LayoutKind.Auto)]
            public readonly struct Edge
            {
                public int From { get; }
                public int To { get; }
                public int Capacity { get; }
                public int Flow { get; }
                public long Cost { get; }

                public Edge(int from, int to, int capacity, int flow, long cost)
                {
                    From = from;
                    To = to;
                    Capacity = capacity;
                    Flow = flow;
                    Cost = cost;
                }

                public override string ToString() => $"{nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Capacity)}: {Capacity}, {nameof(Flow)}: {Flow}, {nameof(Cost)}: {Cost}";
            }

            [StructLayout(LayoutKind.Auto)]
            private readonly struct InternalEdge
            {
                public int To { get; }
                public int InvIndex { get; }
                public int Capacity { get; }
                public long Cost { get; }

                public InternalEdge(int to, int invIndex, int capacity, long cost)
                {
                    To = to;
                    InvIndex = invIndex;
                    Capacity = capacity;
                    Cost = cost;
                }

                public InternalEdge Flow(int flow) => new InternalEdge(To, InvIndex, Capacity + flow, Cost);

                public override string ToString() => $"{nameof(To)}: {To}, {nameof(InvIndex)}: {InvIndex}, {nameof(Capacity)}: {Capacity}, {nameof(Cost)}: {Cost}";
            }

            [StructLayout(LayoutKind.Auto)]
            readonly struct DijkstraState : IComparable<DijkstraState>
            {
                public readonly int Vertex;
                public readonly long Distance;

                public DijkstraState(int v, long distance)
                {
                    Vertex = v;
                    Distance = distance;
                }

                public int CompareTo(DijkstraState other) => Distance.CompareTo(other.Distance);
                public void Deconstruct(out int v, out long distance) => (v, distance) = (Vertex, Distance);
                public override string ToString() => $"{nameof(Vertex)}: {Vertex}, {nameof(Distance)}: {Distance}";
            }
        }

        /// <summary>
        /// LCAを求めるクラス。
        /// </summary>
        public class LowestCommonAncester
        {
            readonly int[,] _parents;
            readonly int[] _depths;
            readonly int _ceilLog2;

            public ReadOnlySpan<int> Depths => _depths;

            public LowestCommonAncester(BasicGraph graph, int root = 0)
            {
                if (graph.NodeCount == 0)
                {
                    throw new ArgumentException();
                }

                _ceilLog2 = BitOperations.Log2((uint)(graph.NodeCount - 1)) + 1;
                _parents = new int[_ceilLog2, graph.NodeCount];
                for (int i = 0; i < _ceilLog2; i++)
                {
                    for (int j = 0; j < graph.NodeCount; j++)
                    {
                        _parents[i, j] = -1;
                    }
                }
                _depths = new int[graph.NodeCount];
                _depths.AsSpan().Fill(-1);

                Dfs(root, -1, 0);
                Initialize();

                void Dfs(int current, int parent, int depth)
                {
                    _parents[0, current] = parent;
                    _depths[current] = depth;

                    foreach (var next in graph[current])
                    {
                        if (next != parent)
                        {
                            Dfs(next, current, depth + 1);
                        }
                    }
                }

                void Initialize()
                {
                    for (int pow = 0; pow + 1 < _ceilLog2; pow++)
                    {
                        for (int v = 0; v < _depths.Length; v++)
                        {
                            if (_parents[pow, v] < 0)
                            {
                                _parents[pow + 1, v] = -1;
                            }
                            else
                            {
                                _parents[pow + 1, v] = _parents[pow, _parents[pow, v]];
                            }
                        }
                    }
                }
            }

            public int GetLcaNode(int u, int v)
            {
                if (unchecked((uint)u >= (uint)_depths.Length || (uint)v >= (uint)_depths.Length))
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (_depths[u] < _depths[v])
                {
                    (u, v) = (v, u);
                }

                for (int pow = 0; pow < _ceilLog2; pow++)
                {
                    var depthDiff = _depths[u] - _depths[v];
                    if (((depthDiff >> pow) & 1) > 0)
                    {
                        u = _parents[pow, u];
                    }
                }

                if (u == v)
                {
                    return u;
                }
                else
                {
                    for (int pow = _ceilLog2 - 1; pow >= 0; pow--)
                    {
                        if (_parents[pow, u] != _parents[pow, v])
                        {
                            u = _parents[pow, u];
                            v = _parents[pow, v];
                        }
                    }

                    return _parents[0, u];
                }
            }
        }

        /// <summary>
        /// HLD
        /// </summary>
        public class HeavyLightDecomposition
        {
            private readonly int[] _parents;
            private readonly int[] _sizes;
            private readonly int[] _leaders;
            private readonly int[] _depths;
            private readonly int[] _indice;

            public ReadOnlySpan<int> Parents => _parents;

            /// <summary>
            /// 頂点iがHL分解後のどのインデックスにいるか
            /// </summary>
            public ReadOnlySpan<int> Indice => _indice;

            public HeavyLightDecomposition(BasicGraph tree) : this(tree, 0) { }

            public HeavyLightDecomposition(BasicGraph tree, int root)
            {
                var n = tree.NodeCount;
                _parents = new int[n];
                _sizes = new int[n];
                _leaders = new int[n];
                _depths = new int[n];
                _indice = new int[n];

                _parents[root] = -1;
                SizeDfs(root, -1);

                var index = 0;
                Decompose(root, root, 0, ref index);

                int SizeDfs(int current, int parent)
                {
                    _parents[current] = parent;
                    var size = 1;

                    foreach (var child in tree[current])
                    {
                        if (child == parent)
                        {
                            continue;
                        }

                        size += SizeDfs(child, current);
                    }

                    return _sizes[current] = size;
                }

                void Decompose(int current, int leader, int depth, ref int index)
                {
                    _indice[current] = index++;
                    _leaders[current] = leader;
                    _depths[current] = depth;

                    var maxSize = 0;
                    var maxChild = -1;

                    foreach (var child in tree[current])
                    {
                        if (child == _parents[current])
                        {
                            continue;
                        }

                        if (maxSize.ChangeMax(_sizes[child]))
                        {
                            maxChild = child;
                        }
                    }

                    if (maxChild == -1)
                    {
                        return;
                    }

                    // Heavy-path
                    Decompose(maxChild, leader, depth + 1, ref index);

                    // Light-path
                    foreach (var child in tree[current])
                    {
                        if (child == _parents[current] || child == maxChild)
                        {
                            continue;
                        }

                        Decompose(child, child, depth + 1, ref index);
                    }
                }
            }

            /// <summary>
            /// 頂点<paramref name="u"/>と頂点<paramref name="v"/>間のパスに対応する頂点集合の半開区間[l, r)を列挙する。
            /// </summary>
            public IEnumerable<(int l, int r)> VertexQuery(int u, int v)
            {
                while (_leaders[u] != _leaders[v])
                {
                    if (_indice[u] > _indice[v])
                    {
                        (u, v) = (v, u);
                    }

                    yield return (_indice[_leaders[v]], _indice[v] + 1);
                    v = _parents[_leaders[v]];
                }

                var x = _indice[u];
                var y = _indice[v];

                if (x > y)
                {
                    (x, y) = (y, x);
                }

                yield return (x, y + 1);
            }

            /// <summary>
            /// 頂点<paramref name="u"/>と頂点<paramref name="v"/>間のパスに対応する辺集合の半開区間[l, r)を列挙する。
            /// </summary>
            public IEnumerable<(int l, int r)> EdgeQuery(int u, int v)
            {
                while (_leaders[u] != _leaders[v])
                {
                    if (_indice[u] > _indice[v])
                    {
                        (u, v) = (v, u);
                    }

                    yield return (_indice[_leaders[v]], _indice[v] + 1);
                    v = _parents[_leaders[v]];
                }

                var x = _indice[u];
                var y = _indice[v];

                if (x > y)
                {
                    (x, y) = (y, x);
                }

                // LCA部分を抜く
                yield return (x + 1, y + 1);
            }

            public int GetLca(int u, int v)
            {
                while (true)
                {
                    if (_indice[u] > _indice[v])
                    {
                        (u, v) = (v, u);
                    }

                    if (_leaders[u] == _leaders[v])
                    {
                        return u;
                    }

                    v = _parents[_leaders[v]];
                }
            }

            public int GetDistance(int u, int v) => _depths[u] + _depths[v] - 2 * _depths[GetLca(u, v)];
        }

        public interface ITreeDpState<TSet> : IMonoid<TSet> where TSet : ITreeDpState<TSet>, new()
        {
            public TSet AddRoot();
        }

        public class Rerooting<TEdge, TDp> where TEdge : IEdge where TDp : unmanaged, ITreeDpState<TDp>
        {
            readonly IGraph<TEdge> _graph;
            readonly TDp _identity;
            readonly TDp[][] _dp;
            readonly TDp[] _result;

            public Rerooting(IGraph<TEdge> graph)
            {
                _graph = graph;
                _identity = new TDp().Identity;
                _dp = new TDp[graph.NodeCount][];
                _result = new TDp[_graph.NodeCount];
                _result.AsSpan().Fill(_identity);
            }

            public TDp[] Solve()
            {
                DepthFirstSearch(0, -1);
                Reroot(0, -1, _identity);
                return _result;
            }

            private TDp DepthFirstSearch(int current, int parent)
            {
                var sum = _identity;
                _dp[current] = new TDp[_graph[current].Length];

                var edges = _graph[current];

                for (int i = 0; i < edges.Length; i++)
                {
                    var next = edges[i].To;
                    if (next == parent)
                        continue;
                    _dp[current][i] = DepthFirstSearch(next, current);
                    sum = sum.Merge(_dp[current][i]);
                }

                return sum.AddRoot();
            }

            private void Reroot(int current, int parent, TDp toAdd)
            {
                var edges = _graph[current];
                for (int i = 0; i < edges.Length; i++)
                {
                    if (edges[i].To == parent)
                    {
                        _dp[current][i] = toAdd;
                        break;
                    }
                }

                var dp = GetPrefixSum(current, edges);

                for (int i = 0; i < edges.Length; i++)
                {
                    var next = edges[i].To;
                    if (next == parent)
                        continue;
                    Reroot(next, current, dp[i].AddRoot());
                }
            }

            private TDp[] GetPrefixSum(int root, ReadOnlySpan<TEdge> edges)
            {
                const int StackallocLimit = 512;
                
                // 左右からの累積和
                int sumSize = edges.Length + 1;
                Span<TDp> sumLeft = sumSize <= StackallocLimit ? stackalloc TDp[sumSize] : new TDp[sumSize];
                Span<TDp> sumRight = sumSize <= StackallocLimit ? stackalloc TDp[sumSize] : new TDp[sumSize];

                sumLeft[0] = _identity;
                for (int i = 0; i < edges.Length; i++)
                {
                    sumLeft[i + 1] = sumLeft[i].Merge(_dp[root][i]);
                }

                sumRight[^1] = _identity;
                for (int i = edges.Length - 1; i >= 0; i--)
                {
                    sumRight[i] = sumRight[i + 1].Merge(_dp[root][i]);
                }

                // 頂点vの答え
                _result[root] = sumLeft[^1].AddRoot();

                // 頂点iを除いた累積
                var dp = new TDp[edges.Length];
                for (int i = 0; i < dp.Length; i++)
                {
                    dp[i] = sumLeft[i].Merge(sumRight[i + 1]);
                }

                return dp;
            }
        }
    }
}

#endregion
