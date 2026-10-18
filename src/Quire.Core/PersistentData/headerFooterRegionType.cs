#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class headerFooterRegionType
{
    [MirrorVerified("PtcPersistentDataheaderFooterRegionTypeMechanicalDifferentialTests")]
    public headerFooterRegionType() { }

    [MirrorVerified("PtcPersistentDataheaderFooterRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataheaderFooterRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataheaderFooterRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDataheaderFooterRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }
}
