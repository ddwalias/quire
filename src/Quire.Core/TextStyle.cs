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

    [MirrorVerified("Release IL")]
    public TextStyle(string key, Quire.Core.TextFormat textFormat, bool reset = false) : base(textFormat, reset)
    {
        StyleKey = key ?? StyleKey;
    }
}
