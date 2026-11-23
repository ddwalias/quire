#nullable disable
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial struct av
{
    [MirrorVerified("Release IL")]
    public readonly bool b;

    [MirrorVerified("Release IL")]
    public readonly au[] c;

    [MirrorVerified("Release IL")]
    public av(bool flag, params au[] secondItems)
    {
        b = flag;
        c = secondItems;
    }
}
