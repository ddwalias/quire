#nullable disable
using System.Collections;
using System.Collections.Specialized;
using Quire.Wpf;

namespace Quire;

public static partial class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
    {
        if (items == null)
        {
            goto NullItems;
        }
        if (items is string value)
        {
            goto StringItems;
        }
        ICollection collection = items as ICollection;
        if (collection == null)
        {
            goto OtherItems;
        }
        goto CollectionItems;

    NullItems:
        return true;

    StringItems:
        return string.IsNullOrEmpty(value);

    CollectionItems:
        return collection.Count == 0;

    OtherItems:
        return !items.Any();
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> items) => !IsNullOrEmpty<T>(items);

    public static bool IsNotEmpty<T>(this IEnumerable<T> items, out IList<T> list)
    {
        list = null;
        if (items.IsNotEmpty<T>())
        {
            list = (items as IList<T>) ?? items.ToList();
        }
        return list != null;
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

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

    public static IEnumerable<TRetType> Collect<T, TRetType>(this IEnumerable<T> items, Func<T, TRetType> action) => CollectAsList<T, TRetType>(items, action);

    public static List<TRetType> CollectAsList<T, TRetType>(this IEnumerable<T> items, Func<T, TRetType> action) => Enumerable.ToList<TRetType>(Enumerable.Select<T, TRetType>(items, action));

    public static int IndexOf<T>(this IEnumerable<T> items, T item) => new List<T>(items).IndexOf(item);

    public static void RemoveAll(this IList list)
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
        catch (InvalidOperationException exception)
        {
            DebugTools.FailAssertion(exception.StackTrace);
        }
    }

    public static void RemoveLast(this IList list)
    {
        if (list != null && list.Count > 0 && DebugTools.DevAssert(!list.IsFixedSize && !list.IsReadOnly, null).Passed)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    public static IEnumerable<int> Range(this int max)
    {
        for (int i = 0; i < max; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable<int> Range(this int start, int end)
    {
        int i = start;
        while (i <= end)
        {
            yield return i;
            int num = i + 1;
            i = num;
        }
    }

}
