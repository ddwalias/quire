#nullable disable
using System.Text;
using Quire.Mirror;

namespace Quire;

public static partial class StringBuilderExtensions
{
    [MirrorVerified("Release IL")]
    public static void AppendReverse(this StringBuilder stringBuilder, string text) => AppendReverse(stringBuilder, text, text.Length, text.Length - 1);
}
