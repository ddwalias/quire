#nullable disable
using Quire.Controls.Core.MC14Conversion;
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial class SerializationHelper
{
    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string ApplicationName { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Version AppVersion { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string Release { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Version FormatVersion { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string EngineVersion { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    private readonly Dictionary<string, Mathcad14TextStyle> _textStyles;

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Dictionary<string, Mathcad14TextStyle> MC14TextStyles => _textStyles;

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> MainRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> HeaderRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FooterRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FirstPageHeaderRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FirstPageFooterRegions { get; set; }

    partial class BytesComparer
    {
        [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
        public BytesComparer() { }
    }
}
