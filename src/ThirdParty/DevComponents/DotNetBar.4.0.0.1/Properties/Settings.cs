#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon.Properties;

internal partial class Settings
{
    [MirrorVerified("Release IL")]
    public Settings() { }

    [MirrorVerified("Release IL")]
    private static Settings defaultInstance;

    [MirrorVerified("Release IL")]
    public static Settings Default => defaultInstance;
}
