// ここにQuestionクラスをコピペ
using AtCoderTemplateForNetCore.Algorithms;
using AtCoderTemplateForNetCore.Collections;
using AtCoderTemplateForNetCore.Questions;
using AtCoderTemplateForNetCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AtCoderTemplateForNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IAtCoderQuestion question = new QuestionA();    // 問題に合わせて書き換え
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

namespace AtCoderTemplateForNetCore.Algorithms
{
    public static class BasicAlgorithm
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

    public static class StringAlgorithm
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
            CheckNR(n, r);
            return n == r ? new Modular(1, mod) : Factorial(n, mod) / (Factorial(r, mod) * Factorial(n - r, mod));   // n!まで計算済みなので nCr = n! / (n-r)!r! = nPr / n!とするより速い
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
}

#endregion

#region Collections

namespace AtCoderTemplateForNetCore.Collections
{
    // See https://kumikomiya.com/competitive-programming-with-c-sharp/
    public class UnionFindNode<T>
    {
        private int _height;        // rootのときのみ有効
        private int _groupSize;     // 同上
        private UnionFindNode<T> _parent;
        public T Item { get; }

        public UnionFindNode(T item)
        {
            _height = 0;
            _groupSize = 1;
            _parent = this;
            Item = item;
        }

        public UnionFindNode<T> FindRoot()
        {
            if (_parent != this) // not ref equals
            {
                var root = _parent.FindRoot();
                _parent = root;
            }

            return _parent;
        }

        public int GetGroupSize() => FindRoot()._groupSize;

        public void Unite(UnionFindNode<T> other)
        {
            var thisRoot = this.FindRoot();
            var otherRoot = other.FindRoot();

            if (thisRoot == otherRoot)
            {
                return;
            }

            if (thisRoot._height < otherRoot._height)
            {
                thisRoot._parent = otherRoot;
                otherRoot._groupSize += thisRoot._groupSize;
                otherRoot._height = Math.Max(thisRoot._height + 1, otherRoot._height);
            }
            else
            {
                otherRoot._parent = thisRoot;
                thisRoot._groupSize += otherRoot._groupSize;
                thisRoot._height = Math.Max(otherRoot._height + 1, thisRoot._height);
            }
        }

        public bool IsInSameGroup(UnionFindNode<T> other) => this.FindRoot() == other.FindRoot();

        public override string ToString() => $"{Item} root:{FindRoot().Item}";
    }

    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> _heap = new List<T>();
        private readonly int _reverseFactor;
        public int Count => _heap.Count;
        public bool IsDescending => _reverseFactor == 1;

        public PriorityQueue() : this(null, true) { }

        public PriorityQueue(IEnumerable<T> collection) : this(collection, true) { }

        public PriorityQueue(bool descending) : this(null, descending) { }

        public PriorityQueue(IEnumerable<T> collection, bool descending)
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

    public class SegmentTree<T> : IEnumerable<T>
    {
        private readonly T[] _data;
        private readonly T _identityElement;
        private readonly Func<T, T, T> _queryOperation;

        private readonly int _leafOffset;   // n - 1
        private readonly int _leafLength;   // n (= 2^k)

        public int Length { get; }          // 実データ長
        public ReadOnlySpan<T> Data => _data[_leafOffset..(_leafOffset + Length)];

        public SegmentTree(ICollection<T> data, Func<T, T, T> queryOperation, T identityElement)
        {
            Length = data.Count;
            _leafLength = GetMinimumPow2(data.Count);
            _leafOffset = _leafLength - 1;
            _data = new T[_leafOffset + _leafLength];
            _queryOperation = queryOperation;
            _identityElement = identityElement;

            data.CopyTo(_data, _leafOffset);
            BuildTree();
        }

        public T this[int index]
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
                    _data[index] = _queryOperation(_data[index * 2 + 1], _data[index * 2 + 2]);
                }
            }
        }

        public T Query(Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(Length);
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), $"{nameof(range)}の長さは0より大きくなければなりません。");
            }
            return Query(offset, offset + length);
        }

        public T Query(int begin, int end)
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

        private T Query(int begin, int end, int index, int left, int right)
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
                return _queryOperation(leftValue, rightValue);
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
                _data[i] = _queryOperation(_data[2 * i + 1], _data[2 * i + 2]); // f(left, right)
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

        public IEnumerator<T> GetEnumerator()
        {
            var upperIndex = _leafOffset + Length;
            for (int i = _leafOffset; i < upperIndex; i++)
            {
                yield return _data[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
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