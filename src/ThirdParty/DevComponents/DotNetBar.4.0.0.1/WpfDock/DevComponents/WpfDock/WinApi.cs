#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

internal partial class WinApi
{
    [MirrorVerified("Release IL")]
    public static int LOWORD(int n) => (short)(n & 65535);

    [MirrorVerified("Release IL")]
    public static int HIWORD(int n) => (short)(n >> 16 & 65535);

    [MirrorVerified("Release IL")]
    public static int LOWORD(nint n) => LOWORD((int)(long)n);

    [MirrorVerified("Release IL")]
    public static int HIWORD(nint n) => HIWORD((int)(long)n);

    [MirrorVerified("Release IL")]
    public WinApi() { }
}
