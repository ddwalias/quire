#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

    public static int IndexOf<T>(this IEnumerable<T> items, T item) => new List<T>(items).IndexOf(item);

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
