#nullable disable
using Quire.Mirror;

namespace Quire.Properties;

internal partial class Settings
{
    [MirrorVerified("Release IL")]
    public Settings() { }

    [MirrorVerified("Release IL")]
    private static Settings defaultInstance;

    [MirrorVerified("Release IL")]
    public static Settings Default => defaultInstance;

    [MirrorVerified("Release IL")]
    public bool CreateAssertionLogs { get => (bool)this["CreateAssertionLogs"]; set => this["CreateAssertionLogs"] = value; }

    [MirrorVerified("Release IL")]
    public string LicensePath { get => (string)this["LicensePath"]; set => this["LicensePath"] = value; }
}
