#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon.Properties;

public partial class Resources
{
    [MirrorVerified("Release IL")]
    internal Resources() { }

    [MirrorVerified("Release IL")]
    private static CultureInfo resourceCulture;

    [MirrorVerified("Release IL")]
    public static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }
}
