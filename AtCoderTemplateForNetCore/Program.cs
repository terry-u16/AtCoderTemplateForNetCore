// ここにQuestionクラスをコピペ
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (a <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(a), $"{nameof(b)}は正の整数である必要があります。");
            }
            if (b <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(b), $"{nameof(b)}は正の整数である必要があります。");
            }
            if (a < b)
            {
                (a, b) = (b, a);
            }

            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        public static long Lcm(long a, long b)
        {
            if (a <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(a), $"{nameof(b)}は正の整数である必要があります。");
            }
            if (b <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(b), $"{nameof(b)}は正の整数である必要があります。");
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

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    public readonly struct Modular : IEquatable<Modular>, IComparable<Modular>
    {
        private const int _defaultMod = 1000000007;
        public int Value { get; }
        private readonly int? _mod;     // int?にすると遅くなるけど意図せずmod=0とかで初期化されて意味不明な計算結果になるよりはマシ
        public int Mod => _mod ?? throw new InvalidOperationException($"{nameof(Mod)}がnullです。new {nameof(Modular)}(long value, int mod)で初期化してください。");

        public Modular(long value, int mod = _defaultMod)
        {
            if (mod < 2 || mod > 1073741789)
            {
                // 1073741789はint.MaxValue / 2 = 1073741823以下の最大の素数
                throw new ArgumentOutOfRangeException(nameof(mod), $"{nameof(mod)}は2以上1073741789以下の素数でなければなりません。");
            }
            _mod = mod;

            if (value >= 0 && value < mod)
            {
                Value = (int)value;
            }
            else
            {
                Value = (int)(value % mod);
                if (Value < 0)
                {
                    Value += mod;
                }
            }
        }

        public static Modular operator +(in Modular a, in Modular b)
        {
            CheckModEquals(a, b);

            var result = a.Value + b.Value;
            if (result > a.Mod)
            {
                result -= a.Mod;    // 剰余演算を避ける
            }
            return new Modular(result, a.Mod);
        }

        public static Modular operator -(in Modular a, in Modular b)
        {
            CheckModEquals(a, b);

            var result = a.Value - b.Value;
            if (result < 0)
            {
                result += a.Mod;    // 剰余演算を避ける
            }
            return new Modular(result, a.Mod);
        }

        public static Modular operator *(in Modular a, in Modular b)
        {
            CheckModEquals(a, b);
            return new Modular((long)a.Value * b.Value, a.Mod);
        }

        public static Modular operator /(in Modular a, in Modular b)
        {
            CheckModEquals(a, b);
            return a * Pow(b, a.Mod - 2);
        }

        // 需要は不明だけど一応
        public static bool operator ==(in Modular left, in Modular right) => left.Equals(right);
        public static bool operator !=(in Modular left, in Modular right) => !(left == right);
        public static bool operator <(in Modular left, in Modular right) => left.CompareTo(right) < 0;
        public static bool operator <=(in Modular left, in Modular right) => left.CompareTo(right) <= 0;
        public static bool operator >(in Modular left, in Modular right) => left.CompareTo(right) > 0;
        public static bool operator >=(in Modular left, in Modular right) => left.CompareTo(right) >= 0;

        public static explicit operator int(in Modular a) => a.Value;
        public static explicit operator long(in Modular a) => a.Value;

        public static Modular Pow(in Modular a, int n)
        {
            switch (n)
            {
                case 0:
                    return new Modular(1, a.Mod);
                case 1:
                    return a;
                case int m when m >= 0: // ジャンプテーブル化はできなくなる
                    var p = Pow(a, m >> 1);             // m / 2
                    return p * p * Pow(a, m & 0x01);    // m % 2
                default:
                    throw new ArgumentOutOfRangeException(nameof(n), $"べき指数{nameof(n)}は0以上の整数でなければなりません。");
            }
        }

        private static Dictionary<int, List<int>> _factorialCache;
        private static Dictionary<int, List<int>> FactorialCache => _factorialCache ??= new Dictionary<int, List<int>>();
        private static Dictionary<int, int[]> FactorialInverseCache { get; } = new Dictionary<int, int[]>();
        const int maxFactorial = 1000000;

        public static Modular Factorial(int n, int mod = _defaultMod)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)}は0以上の整数でなければなりません。");
            }
            if (mod < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(mod), $"{nameof(mod)}は2以上の素数でなければなりません。");
            }

            if (!FactorialCache.ContainsKey(mod))
            {
                FactorialCache.Add(mod, new List<int>() { 1 });
            }

            var cache = FactorialCache[mod];
            for (int i = cache.Count; i <= n; i++)  // Countが1（0!までキャッシュ済み）のとき1!～n!まで計算
            {
                cache.Add((int)((long)cache[i - 1] * i % mod));
            }
            return new Modular(cache[n], mod);
        }

        public static Modular Permutation(int n, int r, int mod = _defaultMod)
        {
            CheckNR(n, r);
            return Factorial(n, mod) / Factorial(n - r, mod);
        }

        public static Modular Combination(int n, int r, int mod = _defaultMod)
        {
            if (!FactorialInverseCache.ContainsKey(mod))
            {
                InitializeCombinationTable(maxFactorial, mod);
            }
            CheckNR(n, r);
            r = Math.Min(r, n - r);
            return new Modular(FactorialCache[mod][n], mod) * new Modular(FactorialInverseCache[mod][r], mod) * new Modular(FactorialInverseCache[mod][n - r], mod);
        }

        private static void InitializeCombinationTable(int max, int mod)
        {
            Factorial(max);
            FactorialInverseCache.Add(mod, new int[max + 1]);

            long fInv = (new Modular(1, mod) / Factorial(max, mod)).Value;
            FactorialInverseCache[mod][max] = (int)fInv;
            for (int i = max - 1; i >= 0; i--)
            {
                fInv = (fInv * (i + 1)) % mod;
                FactorialInverseCache[mod][i] = (int)fInv;
            }
        }

        public static Modular CombinationWithRepetition(int n, int r, int mod = _defaultMod) => Combination(n + r - 1, r, mod);

        public static Modular[] CreateArray(int length, int mod = _defaultMod) => Enumerable.Repeat(new Modular(0, mod), length).ToArray();

        private static void CheckModEquals(in Modular a, in Modular b)
        {
            if (a.Mod != b.Mod)
            {
                throw new ArgumentException($"{nameof(a)}, {nameof(b)}", $"両者の法{nameof(Mod)}は等しくなければなりません。");
            }
        }

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

        public override string ToString() => $"{Value} (mod {Mod})";

        public override bool Equals(object obj) => obj is Modular m ? Equals(m) : false;

        public bool Equals([System.Diagnostics.CodeAnalysis.AllowNull] Modular other) => Value == other.Value && Mod == other.Mod;

        public int CompareTo([System.Diagnostics.CodeAnalysis.AllowNull] Modular other)
        {
            CheckModEquals(this, other);
            return Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => (Value, Mod).GetHashCode();
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
    public static class StringAlgorithms
    {
        public static int[] ZAlgorithm(string s) => ZAlgorithm(s.AsSpan());

        public static int[] ZAlgorithm(ReadOnlySpan<char> s)
        {
            var z = new int[s.Length];
            z[0] = s.Length;
            var offset = 1;
            var length = 0;

            while (offset < s.Length)
            {
                while (offset + length < s.Length && s[length] == s[offset + length])
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

    public interface ITreeDpState<TSet> : IMonoid<TSet> where TSet : ITreeDpState<TSet>, new()
    {
        public TSet AddRoot();
    }

    public class Rerooting<TTreeDpState> where TTreeDpState : ITreeDpState<TTreeDpState>, new()
    {
        readonly IReadOnlyList<int>[] _graph;
        readonly TTreeDpState _identity;
        readonly Dictionary<int, TTreeDpState>[] _dp;
        readonly TTreeDpState[] _result;

        public Rerooting(IReadOnlyList<int>[] graph)
        {
            _graph = graph;
            _identity = new TTreeDpState().Identity;
            _dp = new Dictionary<int, TTreeDpState>[_graph.Length];
            _result = new TTreeDpState[_graph.Length];
        }

        public TTreeDpState[] Solve()
        {
            DepthFirstSearch();
            Reroot();
            return _result;
        }

        private TTreeDpState DepthFirstSearch() => DepthFirstSearch(0, -1);

        private TTreeDpState DepthFirstSearch(int root, int parent)
        {
            var sum = _identity;
            _dp[root] = new Dictionary<int, TTreeDpState>();

            foreach (var child in _graph[root])
            {
                if (child == parent)
                    continue;
                _dp[root].Add(child, DepthFirstSearch(child, root));
                sum *= _dp[root][child];
            }
            return sum.AddRoot();
        }

        private void Reroot() => Reroot(0, -1, _identity);

        private void Reroot(int root, int parent, TTreeDpState toAdd)
        {
            var degree = _graph[root].Count;
            for (int i = 0; i < _graph[root].Count; i++)
            {
                var child = _graph[root][i];
                if (child == parent)
                {
                    _dp[root].Add(child, toAdd);
                    break;
                }
            }

            // 累積和
            int sumSize = degree + 1;
            var sumLeft = new TTreeDpState[sumSize];
            sumLeft[0] = _identity;
            for (int i = 0; i < degree; i++)
            {
                var child = _graph[root][i];
                sumLeft[i + 1] = sumLeft[i] * _dp[root][child];
            }

            var sumRight = new TTreeDpState[sumSize];
            sumRight[degree] = _identity;
            for (int i = degree - 1; i >= 0; i--)
            {
                var child = _graph[root][i];
                sumRight[i] = sumRight[i + 1] * _dp[root][child];
            }
            _result[root] = sumLeft[degree].AddRoot();

            for (int i = 0; i < _graph[root].Count; i++)
            {
                var child = _graph[root][i];
                if (child == parent)
                    continue;
                var dp = sumLeft[i] * sumRight[i + 1];
                Reroot(child, root, dp.AddRoot());
            }
        }
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
        public ReadOnlySpan<TMonoid> Data => _data[_leafOffset..(_leafOffset + Length)];

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
                if (_innerDictionary.ContainsKey(key))
                {
                    _innerDictionary[key] = value;
                }
                else
                {
                    _innerDictionary.Add(key, value);
                }
            }
        }
    }

    public static class SearchExtensions
    {
        public static Range GetRangeGreaterEqual<T>(this Span<T> span, T minValue) where T : IComparable<T> => GetRangeGreaterEqual((ReadOnlySpan<T>)span, minValue);

        public static Range GetRangeGreaterEqual<T>(this ReadOnlySpan<T> span, T minValue) where T : IComparable<T>
        {
            int ng = -1;
            int ok = span.Length;

            return BoundaryBinarySearch(span, v => v.CompareTo(minValue) >= 0, ng, ok)..;
        }

        public static Range GetRangeSmallerEqual<T>(this Span<T> span, T maxValue) where T : IComparable<T> => GetRangeSmallerEqual((ReadOnlySpan<T>)span, maxValue);

        public static Range GetRangeSmallerEqual<T>(this ReadOnlySpan<T> span, T maxValue) where T : IComparable<T>
        {
            int ng = span.Length;
            int ok = -1;

            return ..(BoundaryBinarySearch(span, v => v.CompareTo(maxValue) <= 0, ng, ok) + 1);
        }

        private static int BoundaryBinarySearch<T>(ReadOnlySpan<T> span, Predicate<T> predicate, int ng, int ok)
        {
            // めぐる式二分探索
            // Span.BinarySearchだとできそうでできない（lower_boundがダメ）
            while (Math.Abs(ok - ng) > 1)
            {
                int mid = (ok + ng) / 2;

                if (predicate(span[mid]))
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

#region Extensions

namespace AtCoderTemplateForNetCore.Extensions
{
    internal static class TextReaderExtensions
    {
        internal static int ReadInt(this TextReader reader) => int.Parse(ReadString(reader));
        internal static long ReadLong(this TextReader reader) => long.Parse(ReadString(reader));
        internal static double ReadDouble(this TextReader reader) => double.Parse(ReadString(reader));
        internal static string ReadString(this TextReader reader) => reader.ReadLine();

        internal static int[] ReadIntArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(int.Parse).ToArray();
        internal static long[] ReadLongArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(long.Parse).ToArray();
        internal static double[] ReadDoubleArray(this TextReader reader, char separator = ' ') => ReadStringArray(reader, separator).Select(double.Parse).ToArray();
        internal static string[] ReadStringArray(this TextReader reader, char separator = ' ') => reader.ReadLine().Split(separator);

        // Supports primitive type only.
        internal static T1 ReadValue<T1>(this TextReader reader) => (T1)Convert.ChangeType(reader.ReadLine(), typeof(T1));

        internal static (T1, T2) ReadValue<T1, T2>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            return (v1, v2);
        }

        internal static (T1, T2, T3) ReadValue<T1, T2, T3>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            return (v1, v2, v3);
        }

        internal static (T1, T2, T3, T4) ReadValue<T1, T2, T3, T4>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            return (v1, v2, v3, v4);
        }

        internal static (T1, T2, T3, T4, T5) ReadValue<T1, T2, T3, T4, T5>(this TextReader reader, char separator = ' ')
        {
            var inputs = ReadStringArray(reader, separator);
            var v1 = (T1)Convert.ChangeType(inputs[0], typeof(T1));
            var v2 = (T2)Convert.ChangeType(inputs[1], typeof(T2));
            var v3 = (T3)Convert.ChangeType(inputs[2], typeof(T3));
            var v4 = (T4)Convert.ChangeType(inputs[3], typeof(T4));
            var v5 = (T5)Convert.ChangeType(inputs[4], typeof(T5));
            return (v1, v2, v3, v4, v5);
        }

        internal static (T1, T2, T3, T4, T5, T6) ReadValue<T1, T2, T3, T4, T5, T6>(this TextReader reader, char separator = ' ')
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

        internal static (T1, T2, T3, T4, T5, T6, T7) ReadValue<T1, T2, T3, T4, T5, T6, T7>(this TextReader reader, char separator = ' ')
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

        internal static (T1, T2, T3, T4, T5, T6, T7, T8) ReadValue<T1, T2, T3, T4, T5, T6, T7, T8>(this TextReader reader, char separator = ' ')
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