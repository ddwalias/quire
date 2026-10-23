#nullable disable
#pragma warning disable
using System.Linq;

namespace Quire.Engine.Misc;

partial class HashSetUtils
{
    public static T[] HashSetToArray<T>(ref global::System.Collections.Generic.HashSet<T> hash) => hash.ToArray();
    public HashSetUtils() { }
}
