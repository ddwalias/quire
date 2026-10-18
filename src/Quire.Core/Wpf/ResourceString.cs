#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ResourceString
{
    [MirrorVerified("PtcWpfResourceStringMechanicalDifferentialTests")]
    public ResourceString() { }

    [MirrorVerified("PtcWpfResourceStringMechanicalDifferentialTests")]
    public string Value { get; set; }

    [MirrorVerified("PtcWpfResourceStringMechanicalDifferentialTests")]
    public string StringFormat { get; set; }

    [MirrorVerified("PtcWpfResourceStringMechanicalDifferentialTests")]
    public string String => ToString();
}
