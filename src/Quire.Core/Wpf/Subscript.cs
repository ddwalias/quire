#nullable disable
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class Subscript
{
    [MirrorVerified("PtcWpfSubscriptMechanicalDifferentialTests")]
    public Subscript() { }

    [MirrorVerified("Release IL")]
    public Subscript(Inline inline)
    {
        base.Inlines.Add(inline);
    }
}
