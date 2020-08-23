using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using AtCoderTemplateForNetCore.Extensions;
using AtCoderTemplateForNetCore.Numerics;
using AtCoderTemplateForNetCore.Questions;

namespace AtCoderTemplateForNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IAtCoderQuestion question = new QuestionA();
            var answers = question.Solve(Console.In);
            
            var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
            Console.SetOut(writer);
            foreach (var answer in answers)
            {
                Console.WriteLine(answer);
            }
            Console.Out.Flush();
        }
    }
}

#region Base Class

namespace AtCoderTemplateForNetCore.Questions
{

    public interface IAtCoderQuestion
    {
        IEnumerable<object> Solve(string input);
        IEnumerable<object> Solve(TextReader inputStream);
    }

    public abstract class AtCoderQuestionBase : IAtCoderQuestion
    {
        public IEnumerable<object> Solve(string input)
        {
            var stream = new MemoryStream(Encoding.Unicode.GetBytes(input));
            var reader = new StreamReader(stream, Encoding.Unicode);

            return Solve(reader);
        }

        public abstract IEnumerable<object> Solve(TextReader inputStream);
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

        public static long Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{n}は0以上の整数でなければなりません。");
            }

            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        public static long Permutation(int n, int r)
        {
            CheckNR(n, r);
            long result = 1;
            for (int i = 0; i < r; i++)
            {
                result *= n - i;
            }
            return result;
        }

        public static long Combination(int n, int r)
        {
            CheckNR(n, r);
            r = Math.Min(r, n - r);

            // See https://stackoverflow.com/questions/1838368/calculating-the-amount-of-combinations
            long result = 1;
            for (int i = 1; i <= r; i++)
            {
                result *= n--;
                result /= i;
            }
            return result;
        }

        public static long CombinationWithRepetition(int n, int r) => Combination(n + r - 1, r);

        public static IEnumerable<(int prime, int count)> PrimeFactorization(int n)
        {
            if (n <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{n}は2以上の整数でなければなりません。");
            }

            var dictionary = new Dictionary<int, int>();
            for (int i = 2; i * i <= n; i++)
            {
                while (n % i == 0)
                {
                    if (dictionary.ContainsKey(i))
                    {
                        dictionary[i]++;
                    }
                    else
                    {
                        dictionary[i] = 1;
                    }

                    n /= i;
                }
            }

            if (n > 1)
            {
                dictionary[n] = 1;
            }

            return dictionary.Select(p => (p.Key, p.Value));
        }

        private static void CheckNR(int n, int r)
        {
            if (n <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は正の整数でなければなりません。");
            }
            if (r < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), $"{nameof(r)}は0以上の整数でなければなりません。");
            }
            if (n < r)
            {
                throw new ArgumentOutOfRangeException($"{nameof(n)},{nameof(r)}", $"{nameof(r)}は{nameof(n)}以下でなければなりません。");
            }
        }
    }

    public readonly struct Modular : IEquatable<Modular>, IComparable<Modular>
    {
        private const int DefaultMod = 1000000007;
        public int Value { get; }
        public static int Mod { get; set; } = DefaultMod;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Modular(long value)
        {
            if (unchecked((ulong)value) < unchecked((ulong)Mod))
            {
                Value = (int)value;
            }
            else
            {
                Value = (int)(value % Mod);
                if (Value < 0)
                {
                    Value += Mod;
                }
            }
        }

        private Modular(int value) => Value = value;
        public static Modular Zero => new Modular(0);
        public static Modular One => new Modular(1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Modular operator +(Modular a, Modular b)
        {
            var result = a.Value + b.Value;
            if (result >= Mod)
            {
                result -= Mod;    // 剰余演算を避ける
            }
            return new Modular(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Modular operator -(Modular a, Modular b)
        {
            var result = a.Value - b.Value;
            if (result < 0)
            {
                result += Mod;    // 剰余演算を避ける
            }
            return new Modular(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Modular operator *(Modular a, Modular b) => new Modular((long)a.Value * b.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Modular operator /(Modular a, Modular b) => a * Pow(b.Value, Mod - 2);

        // 需要は不明だけど一応
        public static bool operator ==(Modular left, Modular right) => left.Equals(right);
        public static bool operator !=(Modular left, Modular right) => !(left == right);
        public static bool operator <(Modular left, Modular right) => left.CompareTo(right) < 0;
        public static bool operator <=(Modular left, Modular right) => left.CompareTo(right) <= 0;
        public static bool operator >(Modular left, Modular right) => left.CompareTo(right) > 0;
        public static bool operator >=(Modular left, Modular right) => left.CompareTo(right) >= 0;

        public static implicit operator Modular(long a) => new Modular(a);
        public static explicit operator int(Modular a) => a.Value;
        public static explicit operator long(Modular a) => a.Value;

        public static Modular Pow(int a, int n)
        {
            if (n == 0)
            {
                return Modular.One;
            }
            else if (n == 1)
            {
                return a;
            }
            else if (n > 0)
            {
                var p = Pow(a, n >> 1);             // m / 2
                return p * p * Pow(a, n & 0x01);    // m % 2
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"べき指数{nameof(n)}は0以上の整数でなければなりません。");
            }
        }

        private static List<int> _factorialCache;
        private static List<int> FactorialCache => _factorialCache ??= new List<int>() { 1 };
        private static int[] FactorialInverseCache { get; set; }
        const int defaultMaxFactorial = 1000000;

        public static Modular Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は0以上の整数でなければなりません。");
            }

            for (int i = FactorialCache.Count; i <= n; i++)  // Countが1（0!までキャッシュ済み）のとき1!～n!まで計算
            {
                FactorialCache.Add((int)((long)FactorialCache[i - 1] * i % Mod));
            }
            return new Modular(FactorialCache[n]);
        }

        public static Modular Permutation(int n, int r)
        {
            CheckNR(n, r);
            return Factorial(n) / Factorial(n - r);
        }

        public static Modular Combination(int n, int r)
        {
            CheckNR(n, r);
            r = Math.Min(r, n - r);
            try
            {
                return new Modular(FactorialCache[n]) * new Modular(FactorialInverseCache[r]) * new Modular(FactorialInverseCache[n - r]);
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException($"{nameof(Combination)}を呼び出す前に{nameof(InitializeCombinationTable)}により前計算を行う必要があります。", ex);
            }
        }

        public static void InitializeCombinationTable(int max = defaultMaxFactorial)
        {
            Factorial(max);
            FactorialInverseCache = new int[max + 1];

            var fInv = (Modular.One / Factorial(max)).Value;
            FactorialInverseCache[max] = fInv;
            for (int i = max - 1; i >= 0; i--)
            {
                fInv = (int)((long)fInv * (i + 1) % Mod);
                FactorialInverseCache[i] = fInv;
            }
        }

        public static Modular CombinationWithRepetition(int n, int r) => Combination(n + r - 1, r);

        private static void CheckNR(int n, int r)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は0以上の整数でなければなりません。");
            }
            if (r < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), $"{nameof(r)}は0以上の整数でなければなりません。");
            }
            if (n < r)
            {
                throw new ArgumentOutOfRangeException($"{nameof(n)},{nameof(r)}", $"{nameof(r)}は{nameof(n)}以下でなければなりません。");
            }
        }

        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => obj is Modular m ? Equals(m) : false;
        public bool Equals([System.Diagnostics.CodeAnalysis.AllowNull] Modular other) => Value == other.Value;
        public int CompareTo([System.Diagnostics.CodeAnalysis.AllowNull] Modular other) => Value.CompareTo(other.Value);
        public override int GetHashCode() => Value.GetHashCode();
    }

    public class ModMatrix
    {
        readonly Modular[] _values;
        public int Height { get; }
        public int Width { get; }

        public Span<Modular> this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _values.AsSpan(row * Width, Width);
        }

        public Modular this[int row, int column]
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
            _values = new Modular[height * width];
        }

        public ModMatrix(Modular[][] values) : this(values.Length, values[0].Length)
        {
            for (int row = 0; row < Height; row++)
            {
                if (Width != values[row].Length)
                    throw new ArgumentException($"{nameof(values)}の列数は揃っている必要があります。");
                var span = _values.AsSpan(row * Width, Width);
                values[row].AsSpan().CopyTo(span);
            }
        }

        public ModMatrix(Modular[,] values) : this(values.GetLength(0), values.GetLength(1))
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

        public ModMatrix(ModMatrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;
            _values = new Modular[matrix._values.Length];
            matrix._values.AsSpan().CopyTo(_values);
        }

        public static ModMatrix GetIdentity(int dimension)
        {
            var result = new ModMatrix(dimension);
            for (int i = 0; i < dimension; i++)
            {
                result._values[i * result.Width + i] = 1;
            }
            return result;
        }

        public static ModMatrix operator +(ModMatrix a, ModMatrix b)
        {
            CheckSameShape(a, b);

            var result = new ModMatrix(a.Height, a.Width);
            for (int i = 0; i < result._values.Length; i++)
            {
                result._values[i] = a._values[i] + b._values[i];
            }
            return result;
        }

        public static ModMatrix operator -(ModMatrix a, ModMatrix b)
        {
            CheckSameShape(a, b);

            var result = new ModMatrix(a.Height, a.Width);
            for (int i = 0; i < result._values.Length; i++)
            {
                result._values[i] = a._values[i] - b._values[i];
            }
            return result;
        }

        public static ModMatrix operator *(ModMatrix a, ModMatrix b)
        {
            if (a.Width != b.Height)
                throw new ArgumentException($"{nameof(a)}の列数と{nameof(b)}の行数は等しくなければなりません。");

            var result = new ModMatrix(a.Height, b.Width);
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

        public static ModVector operator *(ModMatrix matrix, ModVector vector)
        {
            if (matrix.Width != vector.Length)
                throw new ArgumentException($"{nameof(matrix)}の列数と{nameof(vector)}の行数は等しくなければなりません。");

            var result = new ModVector(vector.Length);
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

        public ModMatrix Pow(long pow)
        {
            if (Height != Width)
                throw new ArgumentException("累乗を行う行列は正方行列である必要があります。");
            if (pow < 0)
                throw new ArgumentException($"{nameof(pow)}は0以上の整数である必要があります。");

            var powMatrix = new ModMatrix(this);
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

        private static void CheckSameShape(ModMatrix a, ModMatrix b)
        {
            if (a.Height != b.Height)
                throw new ArgumentException($"{nameof(a)}の行数と{nameof(b)}の行数は等しくなければなりません。");
            else if (a.Width != b.Width)
                throw new ArgumentException($"{nameof(a)}の列数と{nameof(b)}の列数は等しくなければなりません。");
        }

        private void ThrowsArgumentOutOfRangeException(string paramName) => throw new ArgumentOutOfRangeException(paramName);
        public override string ToString() => $"({Height}x{Width})matrix";
    }

    public class ModVector
    {
        readonly Modular[] _values;
        public int Length => _values.Length;

        public ModVector(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            _values = new Modular[length];
        }

        public ModVector(ReadOnlySpan<Modular> vector)
        {
            _values = new Modular[vector.Length];
            vector.CopyTo(_values);
        }

        public ModVector(ModVector vector) : this(vector._values) { }

        public Modular this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }

        public static ModVector operator +(ModVector a, ModVector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = new ModVector(a.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }

        public static ModVector operator -(ModVector a, ModVector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = new ModVector(a.Length);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        public static Modular operator *(ModVector a, ModVector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException($"{nameof(a)}と{nameof(b)}の次元は等しくなければなりません。");

            var result = Modular.Zero;
            for (int i = 0; i < a.Length; i++)
            {
                result += a[i] * b[i];
            }
            return result;
        }

        public override string ToString() => $"({Length})vector";
    }

    public readonly struct Fraction : IEquatable<Fraction>, IComparable<Fraction>
    {
        /// <summary>分子</summary>
        public long Numerator { get; }
        /// <summary>分母</summary>
        public long Denominator { get; }

        public static Fraction Nan => new Fraction(0, 0);
        public static Fraction PositiveInfinity => new Fraction(1, 0);
        public static Fraction NegativeInfinity => new Fraction(-1, 0);
        public bool IsNan => Numerator == 0 && Denominator == 0;
        public bool IsInfinity => Numerator != 0 && Denominator == 0;
        public bool IsPositiveInfinity => Numerator > 0 && Denominator == 0;
        public bool IsNegativeInfinity => Numerator < 0 && Denominator == 0;

        /// <summary>
        /// <c>Fraction</c>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        public Fraction(long numerator, long denominator)
        {
            if (denominator == 0)
            {
                Numerator = Math.Sign(numerator);
                Denominator = 0;
            }
            else if (numerator == 0)
            {
                Numerator = 0;
                Denominator = 1;
            }
            else
            {
                var sign = Math.Sign(numerator) * Math.Sign(denominator);
                numerator = Math.Abs(numerator);
                denominator = Math.Abs(denominator);
                var gcd = NumericalAlgorithms.Gcd(numerator, denominator);
                Numerator = sign * numerator / gcd;
                Denominator = denominator / gcd;
            }
        }

        public static Fraction operator +(in Fraction left, in Fraction right)
        {
            if (left.IsNan || right.IsNan)
            {
                return Nan;
            }
            else if (left.IsInfinity || right.IsInfinity)
            {
                if (!right.IsInfinity)
                {
                    return left;
                }
                else if (!left.IsInfinity)
                {
                    return right;
                }
                else
                {
                    return new Fraction(left.Numerator + right.Numerator, 0);
                }
            }
            else
            {
                var lcm = NumericalAlgorithms.Lcm(left.Denominator, right.Denominator);
                return new Fraction(left.Numerator * (lcm / left.Denominator) + right.Numerator * (lcm / right.Denominator), lcm);
            }
        }
        public static Fraction operator -(in Fraction left, in Fraction right) => left + -right;
        public static Fraction operator *(in Fraction left, in Fraction right) => new Fraction(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        public static Fraction operator /(in Fraction left, in Fraction right) => new Fraction(left.Numerator * right.Denominator, left.Denominator * right.Numerator);
        public static Fraction operator +(in Fraction right) => right;
        public static Fraction operator -(in Fraction right) => new Fraction(-right.Numerator, right.Denominator);
        public static bool operator ==(in Fraction left, in Fraction right) => left.Equals(right);
        public static bool operator !=(in Fraction left, in Fraction right) => !(left == right);
        public static implicit operator double(in Fraction right)
        {
            if (right.IsNan)
            {
                return double.NaN;
            }
            else if (right.IsPositiveInfinity)
            {
                return double.PositiveInfinity;
            }
            else if (right.IsNegativeInfinity)
            {
                return double.NegativeInfinity;
            }
            else
            {
                return (double)right.Numerator / right.Denominator;
            }
        }

        public override string ToString()
        {
            if (IsNan)
            {
                return "NaN";
            }
            else if (IsPositiveInfinity)
            {
                return "Inf";
            }
            else if (IsNegativeInfinity)
            {
                return "-Inf";
            }
            else
            {
                return $"{Numerator}/{Denominator}";
            }
        }

        public override bool Equals(object obj) => obj is Fraction fraction && Equals(fraction);
        public bool Equals(Fraction other) => Numerator == other.Numerator && Denominator == other.Denominator;
        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
        public int CompareTo([System.Diagnostics.CodeAnalysis.AllowNull] Fraction other) => ((double)this).CompareTo(other);
    }

    public interface ISemigroup<TSet> where TSet : ISemigroup<TSet>
    {
        public TSet Multiply(TSet other);
        public static TSet operator *(ISemigroup<TSet> a, TSet b) => a.Multiply(b);
    }

    public interface IMonoid<TSet> : ISemigroup<TSet> where TSet : IMonoid<TSet>, new()
    {
        public TSet Identity { get; }
    }

    public interface IGroup<TSet> : IMonoid<TSet> where TSet : IGroup<TSet>, new()
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
        public static void UpdateWhenSmall<T>(ref T value, T other) where T : IComparable<T>
        {
            if (other.CompareTo(value) < 0)
            {
                value = other;
            }
        }

        public static void UpdateWhenLarge<T>(ref T value, T other) where T : IComparable<T>
        {
            if (other.CompareTo(value) > 0)
            {
                value = other;
            }
        }
    }

    public class CoordinateShrinker<T> : IEnumerable<(int shrinkedIndex, T rawIndex)> where T : IComparable<T>, IEquatable<T>
    {
        Dictionary<T, int> _shrinkMapper;
        T[] _expandMapper;
        public int Count => _expandMapper.Length;

        public CoordinateShrinker(IEnumerable<T> data)
        {
            _expandMapper = data.Distinct().ToArray();
            Array.Sort(_expandMapper);

            _shrinkMapper = new Dictionary<T, int>();
            for (int i = 0; i < _expandMapper.Length; i++)
            {
                _shrinkMapper.Add(_expandMapper[i], i);
            }
        }

        public int Shrink(T rawCoordinate) => _shrinkMapper[rawCoordinate];
        public T Expand(int shrinkedCoordinate) => _expandMapper[shrinkedCoordinate];

        public IEnumerator<(int shrinkedIndex, T rawIndex)> GetEnumerator()
        {
            for (int i = 0; i < _expandMapper.Length; i++)
            {
                yield return (i, _expandMapper[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

#endregion

#region Collections

namespace AtCoderTemplateForNetCore.Collections
{
    // See https://kumikomiya.com/competitive-programming-with-c-sharp/
    public class UnionFindTree
    {
        private UnionFindNode[] _nodes;
        public int Count => _nodes.Length;
        public int Groups { get; private set; }

        public UnionFindTree(int count)
        {
            _nodes = Enumerable.Range(0, count).Select(i => new UnionFindNode(i)).ToArray();
            Groups = _nodes.Length;
        }

        public void Unite(int index1, int index2)
        {
            var succeed = _nodes[index1].Unite(_nodes[index2]);
            if (succeed)
            {
                Groups--;
            }
        }

        public bool IsInSameGroup(int index1, int index2) => _nodes[index1].IsInSameGroup(_nodes[index2]);
        public int GetGroupSizeOf(int index) => _nodes[index].GetGroupSize();

        private class UnionFindNode
        {
            private int _height;        // rootのときのみ有効
            private int _groupSize;     // 同上
            private UnionFindNode _parent;
            public int ID { get; }

            public UnionFindNode(int id)
            {
                _height = 0;
                _groupSize = 1;
                _parent = this;
                ID = id;
            }

            public UnionFindNode FindRoot()
            {
                if (_parent != this) // not ref equals
                {
                    var root = _parent.FindRoot();
                    _parent = root;
                }

                return _parent;
            }

            public int GetGroupSize() => FindRoot()._groupSize;

            public bool Unite(UnionFindNode other)
            {
                var thisRoot = this.FindRoot();
                var otherRoot = other.FindRoot();

                if (thisRoot == otherRoot)
                {
                    return false;
                }

                if (thisRoot._height < otherRoot._height)
                {
                    thisRoot._parent = otherRoot;
                    otherRoot._groupSize += thisRoot._groupSize;
                    otherRoot._height = Math.Max(thisRoot._height + 1, otherRoot._height);
                    return true;
                }
                else
                {
                    otherRoot._parent = thisRoot;
                    thisRoot._groupSize += otherRoot._groupSize;
                    thisRoot._height = Math.Max(otherRoot._height + 1, thisRoot._height);
                    return true;
                }
            }

            public bool IsInSameGroup(UnionFindNode other) => this.FindRoot() == other.FindRoot();

            public override string ToString() => $"{ID} root:{FindRoot().ID}";
        }
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
        private List<T> _heap = new List<T>();
        private readonly int _reverseFactor;
        public int Count => _heap.Count;
        public bool IsDescending => _reverseFactor == 1;

        public PriorityQueue(bool descending) : this(descending, null) { }

        public PriorityQueue(bool descending, IEnumerable<T> collection)
        {
            _reverseFactor = descending ? 1 : -1;
            _heap = new List<T>();

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    Enqueue(item);
                }
            }
        }

        public void Enqueue(T item)
        {
            _heap.Add(item);
            UpHeap();
        }

        public T Dequeue()
        {
            var item = _heap[0];
            DownHeap();
            return item;
        }

        public T Peek() => _heap[0];

        private void UpHeap()
        {
            var child = Count - 1;
            while (child > 0)
            {
                int parent = (child - 1) / 2;

                if (Compare(_heap[child], _heap[parent]) > 0)
                {
                    SwapAt(child, parent);
                    child = parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void DownHeap()
        {
            _heap[0] = _heap[Count - 1];
            _heap.RemoveAt(Count - 1);

            var parent = 0;
            while (true)
            {
                var leftChild = 2 * parent + 1;
                
                if (leftChild > Count - 1)
                {
                    break;
                }

                var target = (leftChild < Count - 1) && (Compare(_heap[leftChild], _heap[leftChild + 1]) < 0) ? leftChild + 1 : leftChild;

                if (Compare(_heap[parent], _heap[target]) < 0)
                {
                    SwapAt(parent, target);
                }
                else
                {
                    break;
                }

                parent = target;
            }
        }

        private int Compare(T a, T b) => _reverseFactor * a.CompareTo(b);

        private void SwapAt(int n, int m) => (_heap[n], _heap[m]) = (_heap[m], _heap[n]);

        public IEnumerator<T> GetEnumerator()
        {
            var copy = new List<T>(_heap);
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
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class SegmentTree<TMonoid> : IEnumerable<TMonoid> where TMonoid : IMonoid<TMonoid>, new()
    {
        private readonly TMonoid[] _data;
        private readonly TMonoid _identityElement;

        private readonly int _leafOffset;   // n - 1
        private readonly int _leafLength;   // n (= 2^k)

        public int Length { get; }          // 実データ長
        public ReadOnlySpan<TMonoid> Data => _data.AsSpan(_leafOffset, Length);

        public SegmentTree(ICollection<TMonoid> data)
        {
            Length = data.Count;
            _leafLength = GetMinimumPow2(data.Count);
            _leafOffset = _leafLength - 1;
            _data = new TMonoid[_leafOffset + _leafLength];
            _identityElement = new TMonoid().Identity;

            data.CopyTo(_data, _leafOffset);
            BuildTree();
        }

        public TMonoid this[int index]
        {
            get => Data[index];
            set
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException($"{nameof(index)}がデータの範囲外です。");
                }
                index += _leafOffset;
                _data[index] = value;
                while (index > 0)
                {
                    // 一つ上の親の更新
                    index = (index - 1) / 2;
                    _data[index] = _data[index * 2 + 1] * _data[index * 2 + 2];
                }
            }
        }

        public TMonoid Query(Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(Length);
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), $"{nameof(range)}の長さは0より大きくなければなりません。");
            }
            return Query(offset, offset + length);
        }

        public TMonoid Query(int begin, int end)
        {
            if (begin < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(begin), $"{nameof(begin)}は0以上の数でなければなりません。");
            }
            if (end > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(end), $"{nameof(end)}は{nameof(Length)}以下でなければなりません。");
            }
            if (begin >= end)
            {
                throw new ArgumentException($"{nameof(begin)},{nameof(end)}", $"{nameof(end)}は{nameof(begin)}より大きい数でなければなりません。");
            }
            return Query(begin, end, 0, 0, _leafLength);
        }

        private TMonoid Query(int begin, int end, int index, int left, int right)
        {
            if (right <= begin || end <= left)      // 範囲外
            {
                return _identityElement;
            }
            else if (begin <= left && right <= end) // 全部含まれる
            {
                return _data[index];
            }
            else    // 一部だけ含まれる
            {
                var leftValue = Query(begin, end, index * 2 + 1, left, (left + right) / 2);     // 左の子
                var rightValue = Query(begin, end, index * 2 + 2, (left + right) / 2, right);   // 右の子
                return leftValue * rightValue;
            }
        }

        private void BuildTree()
        {
            foreach (ref var unusedLeaf in _data.AsSpan()[(_leafOffset + Length)..])
            {
                unusedLeaf = _identityElement;  // 単位元埋め
            }

            for (int i = _leafLength - 2; i >= 0; i--)  // 葉の親から順番に一つずつ上がっていく
            {
                _data[i] = _data[2 * i + 1] * _data[2 * i + 2]; // f(left, right)
            }
        }

        private int GetMinimumPow2(int n)
        {
            var p = 1;
            while (p < n)
            {
                p *= 2;
            }
            return p;
        }

        public IEnumerator<TMonoid> GetEnumerator()
        {
            var upperIndex = _leafOffset + Length;
            for (int i = _leafOffset; i < upperIndex; i++)
            {
                yield return _data[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
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

        public long this[Index index]
        {
            get => Sum(index..(index.GetOffset(Length) + 1));
            set 
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)}は0以上の値である必要があります。");
                }
                AddAt(index, value - this[index]);
            }
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
        public int Count()
        {
            unchecked
            {
                // Hardware Intrinsics未使用
                uint v = _value;
                v = (v & 0x55555555) + (v >> 1 & 0x55555555);
                v = (v & 0x33333333) + (v >> 2 & 0x33333333);
                v = (v & 0x0f0f0f0f) + (v >> 4 & 0x0f0f0f0f);
                v = (v & 0x00ff00ff) + (v >> 8 & 0x00ff00ff);
                v = (v & 0x0000ffff) + (v >> 16 & 0x0000ffff);
                return (int)v;
            }
        }

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

    public static class SearchExtensions
    {
        class LowerBoundComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y) => 0 <= x.CompareTo(y) ? 1 : -1;
        }

        class UpperBoundComparer<T> : IComparer<T> where T : IComparable<T>
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
            // めぐる式二分探索
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

    public static class ArrayExtensions
    {
        public static T[] SetAll<T>(this T[] array, Func<int, T> func)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = func(i);
            return array;
        }

        public static T[,] SetAll<T>(this T[,] array, Func<int, int, T> func)
        {
            var length0 = array.GetLength(0);
            var length1 = array.GetLength(1);
            for (int i = 0; i < length0; i++)
                for (int j = 0; j < length1; j++)
                    array[i, j] = func(i, j);
            return array;
        }

        public static T[,,] SetAll<T>(this T[,,] array, Func<int, int, int, T> func)
        {
            var length0 = array.GetLength(0);
            var length1 = array.GetLength(1);
            var length2 = array.GetLength(2);
            for (int i = 0; i < length0; i++)
                for (int j = 0; j < length1; j++)
                    for (int k = 0; k < length2; k++)
                        array[i, j, k] = func(i, j, k);
            return array;
        }

        public static T[,,,] SetAll<T>(this T[,,,] array, Func<int, int, int, int, T> func)
        {
            var length0 = array.GetLength(0);
            var length1 = array.GetLength(1);
            var length2 = array.GetLength(2);
            var length3 = array.GetLength(3);
            for (int i = 0; i < length0; i++)
                for (int j = 0; j < length1; j++)
                    for (int k = 0; k < length2; k++)
                        for (int l = 0; l < length3; l++)
                            array[i, j, k, l] = func(i, j, k, l);
            return array;
        }
    }
}

#endregion

#region Graphs

namespace AtCoderTemplateForNetCore.Graphs
{
    public interface INode
    {
        public int Index { get; }
    }

    public interface IEdge<TNode> where TNode : INode
    {
        TNode From { get; }
        TNode To { get; }
    }

    public interface IWeightedEdge<TNode> : IEdge<TNode> where TNode : INode
    {
        long Weight { get; }
    }

    public interface IGraph<TNode, TEdge> where TEdge : IEdge<TNode> where TNode : INode
    {
        IEnumerable<TEdge> this[TNode node] { get; }
        IEnumerable<TEdge> Edges { get; }
        IEnumerable<TNode> Nodes { get; }
        int NodeCount { get; }
    }

    public interface IWeightedGraph<TNode, TEdge> : IGraph<TNode, TEdge> where TEdge : IWeightedEdge<TNode> where TNode : INode {  }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct BasicNode : INode, IEquatable<BasicNode>
    {
        public int Index { get; }

        public BasicNode(int index)
        {
            Index = index;
        }

        public override string ToString() => Index.ToString();
        public override bool Equals(object obj) => obj is BasicNode node && Equals(node);
        public bool Equals(BasicNode other) => Index == other.Index;
        public override int GetHashCode() => HashCode.Combine(Index);
        public static bool operator ==(BasicNode left, BasicNode right) => left.Equals(right);
        public static bool operator !=(BasicNode left, BasicNode right) => !(left == right);
        public static implicit operator BasicNode(int value) => new BasicNode(value);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct BasicEdge : IEdge<BasicNode>
    {
        public BasicNode From { get; }
        public BasicNode To { get; }

        public BasicEdge(int from, int to)
        {
            From = from;
            To = to;
        }

        public override string ToString() => $"{From}-->{To}";
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct WeightedEdge : IWeightedEdge<BasicNode>
    {
        public BasicNode From { get; }
        public BasicNode To { get; }
        public long Weight { get; }

        public WeightedEdge(int from, int to) : this(from, to, 1) { }

        public WeightedEdge(int from, int to, long weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public override string ToString() => $"{From}--[{Weight}]-->{To}";
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct GridNode : INode, IEquatable<GridNode>
    {
        public int Row { get; }
        public int Column { get; }
        public int Index { get; }

        public GridNode(int row, int column, int width)
        {
            Row = row;
            Column = column;
            Index = row * width + column;
        }

        public override string ToString() => $"({Row}, {Column})";
        public override int GetHashCode() => HashCode.Combine(Row, Column, Index);
        public override bool Equals(object obj) => obj is GridNode node && Equals(node);
        public bool Equals(GridNode other) => Row == other.Row && Column == other.Column && Index == other.Index;
        public void Deconstruct(out int row, out int column) { row = Row; column = Column; }
        public static bool operator ==(GridNode left, GridNode right) => left.Equals(right);
        public static bool operator !=(GridNode left, GridNode right) => !(left == right);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct GridEdge : IEdge<GridNode>
    {
        public GridNode From { get; }
        public GridNode To { get; }

        public GridEdge(GridNode from, GridNode to)
        {
            From = from;
            To = to;
        }

        public override string ToString() => $"({From.Row}, {From.Column})-->({To.Row}, {To.Column})";
    }

    public class BasicGraph : IGraph<BasicNode, BasicEdge>
    {
        private readonly List<BasicEdge>[] _edges;
        public IEnumerable<BasicEdge> this[BasicNode node] => _edges[node.Index];
        public IEnumerable<BasicEdge> Edges => Nodes.SelectMany(node => this[node]);
        public IEnumerable<BasicNode> Nodes => Enumerable.Range(0, NodeCount).Select(i => new BasicNode(i));
        public int NodeCount { get; }

        public BasicGraph(int nodeCount) : this(nodeCount, Enumerable.Empty<BasicEdge>()) { }

        public BasicGraph(int nodeCount, IEnumerable<BasicEdge> edges)
        {
            _edges = Enumerable.Repeat(0, nodeCount).Select(_ => new List<BasicEdge>()).ToArray();
            NodeCount = nodeCount;
            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public BasicGraph(int nodeCount, IEnumerable<IEnumerable<int>> distances)
        {
            _edges = new List<BasicEdge>[nodeCount];

            int i = 0;
            foreach (var row in distances)
            {
                _edges[i] = new List<BasicEdge>(nodeCount);
                int j = 0;
                foreach (var distance in row)
                {
                    if (distance == 1)
                    {
                        _edges[i].Add(new BasicEdge(i, j++));
                    }
                }
                i++;
            }
        }

        public void AddEdge(BasicEdge edge) => _edges[edge.From.Index].Add(edge);
    }

    public class WeightedGraph : IGraph<BasicNode, WeightedEdge>
    {
        private readonly List<WeightedEdge>[] _edges;
        public IEnumerable<WeightedEdge> this[BasicNode node] => _edges[node.Index];
        public IEnumerable<WeightedEdge> Edges => Nodes.SelectMany(node => this[node]);
        public IEnumerable<BasicNode> Nodes => Enumerable.Range(0, NodeCount).Select(i => new BasicNode(i));
        public int NodeCount { get; }

        public WeightedGraph(int nodeCount) : this(nodeCount, Enumerable.Empty<WeightedEdge>()) { }

        public WeightedGraph(int nodeCount, IEnumerable<WeightedEdge> edges)
        {
            _edges = Enumerable.Repeat(0, nodeCount).Select(_ => new List<WeightedEdge>()).ToArray();
            NodeCount = nodeCount;
            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public WeightedGraph(int nodeCount, IEnumerable<IEnumerable<int>> distances)
        {
            _edges = new List<WeightedEdge>[nodeCount];

            int i = 0;
            foreach (var row in distances)
            {
                _edges[i] = new List<WeightedEdge>(nodeCount);
                int j = 0;
                foreach (var distance in row)
                {
                    _edges[i].Add(new WeightedEdge(i, j++, distance));
                }
                i++;
            }
        }

        public void AddEdge(WeightedEdge edge) => _edges[edge.From.Index].Add(edge);
    }

    public class GridGraph : IGraph<GridNode, GridEdge>
    {
        private readonly IReadOnlyList<(int dx, int dy)> _adjacents;
        public int Height { get; }
        public int Width { get; }
        public int NodeCount => Height * Width;

        public IEnumerable<GridEdge> this[GridNode node]
        {
            get
            {
                foreach (var (dx, dy) in _adjacents)
                {
                    var next = new GridNode(node.Row + dx, node.Column + dy, Width);
                    if (CanEnter(next))
                    {
                        yield return new GridEdge(node, next);
                    }
                }
            }
        }

        public IEnumerable<GridEdge> Edges => Nodes.SelectMany(node => this[node]);
        public IEnumerable<GridNode> Nodes => Enumerable.Range(0, Width).SelectMany(x => Enumerable.Range(0, Height).Select(y => new GridNode(x, y, Width)));

        public GridGraph(int height, int width) : this(height, width, new (int dx, int dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) { }

        public GridGraph(int height, int width, IEnumerable<(int dx, int dy)> adjacents)
        {
            Height = height;
            Width = width;
            _adjacents = adjacents.ToArray();
        }

        protected virtual bool CanEnter(GridNode node)
        {
            unchecked
            {
                return (uint)node.Row < Height && (uint)node.Column < Width;
            }
        }
    }

    namespace Algorithms
    {
        // TGraphは派生クラスでいじりたいことがあるのでジェネリクス。
        // TNode, TEdgeはインターフェースで受け取っても動くが、構造体のDevirtualizationを行うため敢えてジェネリクスにしている（型引数長いのでなんとかしたい……）
        public abstract class BfsBase<TGraph, TNode, TEdge, TResult> where TGraph : IGraph<TNode, TEdge> where TEdge : IEdge<TNode> where TNode : INode
        {
            protected readonly TGraph _graph;
            protected bool _completed;

            protected BfsBase(TGraph graph)
            {
                _graph = graph;
            }

            public TResult Search(TNode startNode)
            {
                var todo = new Queue<TNode>();
                var seen = new bool[_graph.NodeCount];
                var cameFrom = new TNode[_graph.NodeCount];
                _completed = false;
                todo.Enqueue(startNode);
                seen[startNode.Index] = true;
                Initialize(startNode);

                while (todo.Count > 0 && !_completed)
                {
                    var current = todo.Dequeue();
                    var isFirstNode = current.Index == startNode.Index;
                    OnPreordering(current, cameFrom[current.Index], isFirstNode);

                    foreach (var edge in _graph[current])
                    {
                        if (seen[edge.To.Index])
                        {
                            continue;
                        }
                        seen[edge.To.Index] = true;
                        cameFrom[edge.To.Index] = edge.From;
                        todo.Enqueue(edge.To);
                    }
                }

                return GetResult();
            }

            protected abstract void Initialize(TNode startNode);
            protected abstract void OnPreordering(TNode current, TNode previous, bool isFirstNode);
            protected abstract TResult GetResult();
        }

        public abstract class DfsBase<TGraph, TNode, TEdge, TResult> where TGraph : IGraph<TNode, TEdge> where TEdge : IEdge<TNode> where TNode : INode
        {
            protected readonly TGraph _graph;
            protected bool _completed;

            protected DfsBase(TGraph graph)
            {
                _graph = graph;
            }

            public TResult Search(TNode startNode)
            {
                var todo = new Stack<TNode>();
                var seen = new bool[_graph.NodeCount];
                var preorderCompleted = new bool[_graph.NodeCount];
                var cameFrom = new TNode[_graph.NodeCount];
                _completed = false;
                todo.Push(startNode);
                seen[startNode.Index] = true;
                Initialize(startNode);

                while (todo.Count > 0 && !_completed)
                {
                    var current = todo.Peek();
                    var isFirstNode = current.Index == startNode.Index;

                    if (!preorderCompleted[current.Index])
                    {
                        // 行きがけ
                        OnPreordering(current, cameFrom[current.Index], isFirstNode);
                        foreach (var edge in _graph[current])
                        {
                            if (seen[edge.To.Index])
                            {
                                continue;
                            }
                            seen[edge.To.Index] = true;
                            cameFrom[edge.To.Index] = edge.From;
                            todo.Push(edge.To);
                        }
                        preorderCompleted[current.Index] = true;
                    }
                    else
                    {
                        // 帰りがけ
                        OnPostordering(current, cameFrom[current.Index], isFirstNode);
                        _ = todo.Pop();
                    }
                }

                return GetResult();
            }

            protected abstract void Initialize(TNode startNode);
            protected abstract void OnPreordering(TNode current, TNode previous, bool isFirstNode);
            protected abstract void OnPostordering(TNode current, TNode previous, bool isFirstNode);
            protected abstract TResult GetResult();
        }

        // 最短経路問題やるだけの問題でIGraphを派生クラスでいじりたいことはほとんどないので普通にインターフェースで受け取る
        public class Dijkstra<TNode, TEdge> where TEdge : IWeightedEdge<TNode> where TNode : INode
        {
            protected readonly IGraph<TNode, TEdge> _graph;

            public Dijkstra(IGraph<TNode, TEdge> graph)
            {
                _graph = graph;
            }

            public long[] GetDistancesFrom(TNode startNode)
            {
                const long Inf = 1L << 60;
                var distances = Enumerable.Repeat(Inf, _graph.NodeCount).ToArray();
                distances[startNode.Index] = 0;
                var todo = new PriorityQueue<State>(false);
                todo.Enqueue(new State(startNode, 0));

                while (todo.Count > 0)
                {
                    var current = todo.Dequeue();
                    if (current.Distance > distances[current.Node.Index])
                    {
                        continue;
                    }

                    foreach (var edge in _graph[current.Node])
                    {
                        var nextDistance = current.Distance + edge.Weight;
                        if (distances[edge.To.Index] > nextDistance)
                        {
                            distances[edge.To.Index] = nextDistance;
                            todo.Enqueue(new State(edge.To, nextDistance));
                        }
                    }
                }

                return distances;
            }

            private readonly struct State : IComparable<State>
            {
                public TNode Node { get; }
                public long Distance { get; }

                public State(TNode node, long distance)
                {
                    Node = node;
                    Distance = distance;
                }

                public int CompareTo(State other) => Distance.CompareTo(other.Distance);
            }
        }

        public class WarshallFloyd<TNode, TEdge> where TEdge : IWeightedEdge<TNode> where TNode : INode
        {
            protected readonly IGraph<TNode, TEdge> _graph;
            const long Inf = 1L << 60;

            public WarshallFloyd(IGraph<TNode, TEdge> graph)
            {
                _graph = graph;
            }

            public long[,] GetDistances()
            {
                var distances = InitializeDistances();
                for (int k = 0; k < _graph.NodeCount; k++)
                {
                    for (int i = 0; i < _graph.NodeCount; i++)
                    {
                        for (int j = 0; j < _graph.NodeCount; j++)
                        {
                            distances[i, j] = Math.Min(distances[i, j], distances[i, k] + distances[k, j]);
                        }
                    }
                }

                // 一応キレイにしておく
                for (int i = 0; i < _graph.NodeCount; i++)
                {
                    for (int j = 0; j < _graph.NodeCount; j++)
                    {
                        if (distances[i, j] >= Inf)
                        {
                            distances[i, j] = long.MaxValue;
                        }
                    }
                }

                return distances;
            }

            private long[,] InitializeDistances()
            {
                var distances = new long[_graph.NodeCount, _graph.NodeCount];

                for (int i = 0; i < _graph.NodeCount; i++)
                {
                    for (int j = 0; j < _graph.NodeCount; j++)
                    {
                        distances[i, j] = Inf;
                    }
                    distances[i, i] = 0;
                }

                foreach (var node in _graph.Nodes)
                {
                    foreach (var edge in _graph.Edges)
                    {
                        distances[edge.From.Index, edge.To.Index] = edge.Weight;
                    }
                }

                return distances;
            }
        }

        public class BellmanFord<TNode, TEdge> where TEdge : IWeightedEdge<TNode> where TNode : INode
        {
            protected readonly List<TEdge> _edges;
            protected readonly int _nodeCount;

            public BellmanFord(IGraph<TNode, TEdge> graph) : this(graph.Edges, graph.NodeCount) { }

            public BellmanFord(IEnumerable<TEdge> edges, int nodeCount)
            {
                _edges = edges.ToList();
                _nodeCount = nodeCount;
            }

            public (long[] distances, bool[] isNegativeCycle) GetDistancesFrom(TNode startNode)
            {
                const long Inf = long.MaxValue >> 1;
                var distances = Enumerable.Repeat(long.MaxValue, _nodeCount).ToArray();
                var isNegativeCycle = new bool[_nodeCount];
                distances[startNode.Index] = 0;

                for (int i = 1; i <= 2 * _nodeCount; i++)
                {
                    foreach (var edge in _edges)
                    {
                        // そもそも出発点に未到達なら無視
                        if (distances[edge.From.Index] < Inf)
                        {
                            if (i <= _nodeCount)
                            {
                                var newCost = distances[edge.From.Index] + edge.Weight;
                                if (distances[edge.To.Index] > newCost)
                                {
                                    distances[edge.To.Index] = newCost;
                                    // N回目に更新されたやつにチェックを付けて、追加でN回伝播させる
                                    if (i == _nodeCount)
                                    {
                                        isNegativeCycle[edge.To.Index] = true;
                                    }
                                }
                            }
                            else if (isNegativeCycle[edge.From.Index])
                            {
                                isNegativeCycle[edge.To.Index] = true;
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
        }

        public interface ITreeDpState<TSet> : IMonoid<TSet> where TSet : ITreeDpState<TSet>, new()
        {
            public TSet AddRoot();
        }

        public class Rerooting<TNode, TEdge, TTreeDpState> where TEdge : IEdge<TNode> where TNode : struct, INode where TTreeDpState : ITreeDpState<TTreeDpState>, new()
        {
            readonly IGraph<TNode, TEdge> _graph;
            readonly TTreeDpState _identity;
            readonly Dictionary<int, TTreeDpState>[] _dp;
            readonly TTreeDpState[] _result;

            public Rerooting(IGraph<TNode, TEdge> graph)
            {
                _graph = graph;
                _identity = new TTreeDpState().Identity;
                _dp = new Dictionary<int, TTreeDpState>[_graph.NodeCount];
                _result = new TTreeDpState[_graph.NodeCount];
            }

            public TTreeDpState[] Solve()
            {
                DepthFirstSearch(_graph.Nodes.First(), null);
                Reroot(_graph.Nodes.First(), null, _identity);
                return _result;
            }

            private TTreeDpState DepthFirstSearch(TNode root, TNode? parent)
            {
                var sum = _identity;
                _dp[root.Index] = new Dictionary<int, TTreeDpState>();

                foreach (var edge in _graph[root])
                {
                    if (edge.To.Index == parent?.Index)
                        continue;
                    _dp[root.Index].Add(edge.To.Index, DepthFirstSearch(edge.To, root));
                    sum *= _dp[root.Index][edge.To.Index];
                }
                return sum.AddRoot();
            }

            private void Reroot(TNode root, TNode? parent, TTreeDpState toAdd)
            {
                var edges = _graph[root].ToArray();

                foreach (var edge in edges)
                {
                    if (edge.To.Index == parent?.Index)
                    {
                        _dp[root.Index].Add(edge.To.Index, toAdd);
                        break;
                    }
                }

                var dp = GetPrefixSum(root, edges);

                for (int i = 0; i < edges.Length; i++)
                {
                    var child = edges[i].To;
                    if (child.Index == parent?.Index)
                        continue;
                    Reroot(child, root, dp[i].AddRoot());
                }
            }

            private TTreeDpState[] GetPrefixSum(TNode root, TEdge[] edges)
            {
                // 左右からの累積和
                int sumSize = edges.Length + 1;
                var sumLeft = new TTreeDpState[sumSize];
                sumLeft[0] = _identity;
                for (int i = 0; i < edges.Length; i++)
                {
                    var child = edges[i].To;
                    sumLeft[i + 1] = sumLeft[i] * _dp[root.Index][child.Index];
                }

                var sumRight = new TTreeDpState[sumSize];
                sumRight[^1] = _identity;
                for (int i = edges.Length - 1; i >= 0; i--)
                {
                    var child = edges[i].To;
                    sumRight[i] = sumRight[i + 1] * _dp[root.Index][child.Index];
                }

                _result[root.Index] = sumLeft[^1].AddRoot();

                // 頂点iを除いた累積
                var dp = new TTreeDpState[edges.Length];
                for (int i = 0; i < dp.Length; i++)
                {
                    dp[i] = sumLeft[i] * sumRight[i + 1];
                }

                return dp;
            }
        }
    }
}

#endregion

#region Extensions

namespace AtCoderTemplateForNetCore.Extensions
{
    public static class StringExtensions
    {
        public static string Join<T>(this IEnumerable<T> source) => string.Concat(source);
        public static string Join<T>(this IEnumerable<T> source, char separator) => string.Join(separator, source);
        public static string Join<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);
    }

    public static class TextReaderExtensions
    {
        public static int ReadInt(this TextReader reader) => int.Parse(ReadString(reader));
        public static long ReadLong(this TextReader reader) => long.Parse(ReadString(reader));
        public static double ReadDouble(this TextReader reader) => double.Parse(ReadString(reader));
        public static string ReadString(this TextReader reader) => reader.ReadLine();

        public static int[] ReadIntArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(int.Parse).ToArray();
        public static long[] ReadLongArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(long.Parse).ToArray();
        public static double[] ReadDoubleArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(double.Parse).ToArray();
        public static string[] ReadStringArray(this TextReader reader, char separator = ' ') => reader.ReadLine().Split(separator);

        // Supports primitive type only.
        public static T1 ReadValue<T1>(this TextReader reader) => (T1)Convert.ChangeType(reader.ReadLine(), typeof(T1));

        public static (T1, T2) ReadValue<T1, T2>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            return (v1, v2);
        }

        public static (T1, T2, T3) ReadValue<T1, T2, T3>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            return (v1, v2, v3);
        }

        public static (T1, T2, T3, T4) ReadValue<T1, T2, T3, T4>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            return (v1, v2, v3, v4);
        }

        public static (T1, T2, T3, T4, T5) ReadValue<T1, T2, T3, T4, T5>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            var v5 = (T5)Convert.ChangeType(inputs[4], typeof(T5));
            return (v1, v2, v3, v4, v5);
        }

        public static (T1, T2, T3, T4, T5, T6) ReadValue<T1, T2, T3, T4, T5, T6>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            var v5 = (T5)Convert.ChangeType(inputs[4], typeof(T5));
            var v6 = (T6)Convert.ChangeType(inputs[5], typeof(T6));
            return (v1, v2, v3, v4, v5, v6);
        }

        public static (T1, T2, T3, T4, T5, T6, T7) ReadValue<T1, T2, T3, T4, T5, T6, T7>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            var v5 = (T5)Convert.ChangeType(inputs[4], typeof(T5));
            var v6 = (T6)Convert.ChangeType(inputs[5], typeof(T6));
            var v7 = (T7)Convert.ChangeType(inputs[6], typeof(T7));
            return (v1, v2, v3, v4, v5, v6, v7);
        }

        public static (T1, T2, T3, T4, T5, T6, T7, T8) ReadValue<T1, T2, T3, T4, T5, T6, T7, T8>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            var v5 = (T5)Convert.ChangeType(inputs[4], typeof(T5));
            var v6 = (T6)Convert.ChangeType(inputs[5], typeof(T6));
            var v7 = (T7)Convert.ChangeType(inputs[6], typeof(T7));
            var v8 = (T8)Convert.ChangeType(inputs[7], typeof(T8));
            return (v1, v2, v3, v4, v5, v6, v7, v8);
        }
    }
}

#endregion