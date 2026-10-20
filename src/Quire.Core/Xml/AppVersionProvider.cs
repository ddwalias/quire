#nullable disable
using Quire.Mirror;

namespace Quire.Xml;

public static partial class AppVersionProvider
{
    [MirrorVerified("Release IL")]
    public static Version CurrentAppVersion => new Version(VersionData.CurrentAppVersion);

    [MirrorVerified("Release IL")]
    public static Version CurrentFormatVersion => new Version(VersionData.CurrentFormatVersion);

    [MirrorVerified("Release IL")]
    public static Version CurrentEngNbVersion => new Version(VersionData.CurrentEngNbVersion);
}
