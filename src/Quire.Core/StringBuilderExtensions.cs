#nullable disable
using System;
using System.Text;
using Quire.Mirror;

namespace Quire;

public static partial class StringBuilderExtensions
{
    [MirrorVerified("Release IL")]
    public static void AppendReverse(this StringBuilder stringBuilder, string text) => AppendReverse(stringBuilder, text, text.Length, text.Length - 1);

    [MirrorVerified("Release IL")]
    public static void AppendReverse(this StringBuilder stringBuilder, string text, int startIndex, int count)
    {
        if (DebugTools.DevAssert(startIndex >= 0 && count >= 0 && startIndex + count <= text.Length, null).Failed)
        {
            return;
        }
        char[] array = new char[count];
        text.CopyTo(startIndex, array, 0, count);
        Array.Reverse(array);
        stringBuilder.Append(array);
    }

    [MirrorVerified("Release IL")]
    public static string ToStringReverse(this StringBuilder stringBuilder)
    {
        char[] array = new char[stringBuilder.Length];
        stringBuilder.CopyTo(0, array, 0, array.Length);
        Array.Reverse(array);
        return new string(array);
    }
}
