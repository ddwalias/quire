#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using System.Collections.Specialized;
using Quire.Wpf;

namespace Quire;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

    public static int IndexOf<T>(this IEnumerable<T> items, T item) => new List<T>(items).IndexOf(item);

    public static bool IsNotEmpty<T>(this IEnumerable<T> items, out IList<T> list)
    {
        list = null;
        if (items.IsNotEmpty<T>())
        {
            list = (items as IList<T>) ?? items.ToList();
        }
        return list != null;
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

    public static void Each<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items)
        {
            action(item);
        }
    }

    public static void EachWithIndex<T>(this IEnumerable<T> items, Action<int, T> action)
    {
        int num = 0;
        foreach (T item in items)
        {
            action(num++, item);
        }
    }

    public static void RemoveLast(this System.Collections.IList list)
    {
        if (list != null && list.Count > 0 && DebugTools.DevAssert(!list.IsFixedSize && !list.IsReadOnly, null).Passed)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    public static void RemoveAll(this System.Collections.IList list)
    {
        try
        {
            if (list is INotifyCollectionChanged && !(list is INotifyCollectionRangeChanged))
            {
                while (list.Count > 0)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else if (list.Count > 0)
            {
                list.Clear();
            }
        }
        catch (InvalidOperationException ex)
        {
            DebugTools.FailAssertion(ex.StackTrace);
        }
    }

    [MirrorVerified("PtcEnumerableExtensionsMechanicalDifferentialTests")]
    public static IEnumerable<TRetType> Collect<T, TRetType>(this IEnumerable<T> items, Func<T, TRetType> action) => CollectAsList<T, TRetType>(items, action);

    [MirrorVerified("Release IL")]
    public static bool IsNotEmpty<T>(this IEnumerable<T> items) => !IsNullOrEmpty<T>(items);

    [MirrorVerified("Release IL")]
    public static List<TRetType> CollectAsList<T, TRetType>(this IEnumerable<T> items, Func<T, TRetType> action) => Enumerable.ToList<TRetType>(Enumerable.Select<T, TRetType>(items, action));

    [MirrorVerified("Release IL")]
    public static void Times(this int i, Action<int> action) => Each<int>(Range(i), action);

    [MirrorVerified("Release IL")]
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => MaxBy<TSource, TKey>(source, selector, Comparer<TKey>.Default);

    [MirrorVerified("Release IL")]
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) => MinBy<TSource, TKey>(source, selector, Comparer<TKey>.Default);
}
