#nullable disable
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class q
{
    [MirrorVerified("Release IL")]
    public q(p p)
    {
        a_ = p;
    }

    [MirrorVerified("Release IL")]
    public q(p p, string text) : base(text)
    {
        a_ = p;
    }

    [MirrorVerified("Release IL")]
    public p a() => a_;
}
