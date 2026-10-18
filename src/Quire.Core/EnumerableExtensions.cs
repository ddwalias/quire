#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class EnumerableExtensions
{
    [MirrorVerified("PtcEnumerableExtensionsMechanicalDifferentialTests")]
    public static IEnumerable<TRetType> Collect<T, TRetType>(this IEnumerable<T> items, Func<T, TRetType> action) => CollectAsList<T, TRetType>(items, action);
}
