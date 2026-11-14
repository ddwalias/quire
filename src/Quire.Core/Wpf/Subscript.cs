#nullable disable
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class Subscript
{
    static Subscript()
    {
        InlineExtensions.IsSearchScopePropertyKey.OverrideMetadata(typeof(Subscript), new global::System.Windows.PropertyMetadata(true));
    }

    [MirrorVerified("PtcWpfSubscriptMechanicalDifferentialTests")]
    public Subscript() { }

    [MirrorVerified("Release IL")]
    public Subscript(Inline inline)
    {
        base.Inlines.Add(inline);
    }
}
