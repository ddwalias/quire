#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class NullableExtensions
{
    [MirrorVerified("Release IL")]
    public static bool GetValueOrDefault<T>(this T? t, out T result) where T : struct
    {
        result = t.GetValueOrDefault();
        return t.HasValue;
    }
}
