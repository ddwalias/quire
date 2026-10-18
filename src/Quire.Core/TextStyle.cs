#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire;

public partial class TextStyle
{
    [MirrorVerified("PtcTextStyleMechanicalDifferentialTests")]
    public TextStyle(TextStyleData data, bool basedOnValues = true) : base(data, basedOnValues) { }

    [MirrorVerified("PtcTextStyleMechanicalDifferentialTests")]
    public TextStyle(Style style) : base(style) { }

    [MirrorVerified("PtcTextStyleMechanicalDifferentialTests")]
    public TextStyle(string name) : base(name) { }

    [MirrorVerified("PtcTextStyleMechanicalDifferentialTests")]
    public TextStyle(string name, string key) : base(name, key) { }
}
