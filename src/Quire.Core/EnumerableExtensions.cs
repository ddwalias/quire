#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using System.Collections.Specialized;
using Quire.Wpf;

namespace Quire;

public static partial class EnumerableExtensions
{
    [MirrorVerified("Release IL")]
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
    {
        using IEnumerator<TSource> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("Sequence was empty");
        }
        TSource value = enumerator.Current;
        TKey key = selector(value);
        while (enumerator.MoveNext())
        {
            TSource current = enumerator.Current;
            TKey currentKey = selector(current);
            if (comparer.Compare(currentKey, key) > 0)
            {
                value = current;
                key = currentKey;
            }
        }
        return value;
    }

    [MirrorVerified("Release IL")]
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
    {
        using IEnumerator<TSource> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("Sequence was empty");
        }
        TSource value = enumerator.Current;
        TKey key = selector(value);
        while (enumerator.MoveNext())
        {
            TSource current = enumerator.Current;
            TKey currentKey = selector(current);
            if (comparer.Compare(currentKey, key) < 0)
            {
                value = current;
                key = currentKey;
            }
        }
        return value;
    }

    public static void Times(this int count, Action action)
    {
        for (int i = 0; i < count; i++)
        {
            action();
        }
    }

    public static void UpTo(this int start, int end, Action<int> action)
    {
        for (int i = start; i <= end; i++)
        {
            action(i);
        }
    }

    public static void DownTo(this int start, int end, Action<int> action)
    {
        for (int i = start; i >= end; i--)
        {
            action(i);
        }
    }

    public static bool HasCommonValue<T>(this IEnumerable<T> values, out T commonValue)
    {
        commonValue = default;
        if (values.IsNullOrEmpty())
        {
            return false;
        }
        bool flag = false;
        foreach (T value in values)
        {
            if (flag)
            {
                if (!object.Equals(commonValue, value))
                {
                    return false;
                }
            }
            else
            {
                flag = true;
                commonValue = value;
            }
        }
        return true;
    }

    public static TValue GetCommonValue<TObject, TValue>(this IEnumerable<TObject> items, Func<TObject, TValue> getState, TValue defaultValue)
    {
        IEnumerator<TValue> enumerator = items.Select(getState).Distinct().GetEnumerator();
        TValue result = defaultValue;
        if (enumerator.MoveNext())
        {
            result = enumerator.Current;
            if (enumerator.MoveNext())
            {
                result = defaultValue;
            }
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    public static void Times(this int i, Action<int> action) => Each<int>(Range(i), action);

    [MirrorVerified("Release IL")]
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => MaxBy<TSource, TKey>(source, selector, Comparer<TKey>.Default);

    [MirrorVerified("Release IL")]
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => MinBy<TSource, TKey>(source, selector, Comparer<TKey>.Default);
}
