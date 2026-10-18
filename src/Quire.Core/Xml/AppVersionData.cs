#nullable disable
using Quire.Mirror;

namespace Quire.Xml;

public partial class AppVersionData
{
    [MirrorVerified("PtcXmlAppVersionDataMechanicalDifferentialTests")]
    public AppVersionData() { }

    [MirrorVerified("PtcXmlAppVersionDataMechanicalDifferentialTests")]
    public string CurrentAppVersion { get; set; }

    [MirrorVerified("PtcXmlAppVersionDataMechanicalDifferentialTests")]
    public string CurrentFormatVersion { get; set; }

    [MirrorVerified("PtcXmlAppVersionDataMechanicalDifferentialTests")]
    public string CurrentEngNbVersion { get; set; }
}
