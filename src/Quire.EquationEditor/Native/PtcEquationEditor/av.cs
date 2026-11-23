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
    public av(bool A_0, params au[] A_1)
    {
        b = A_0;
        c = A_1;
    }
}
