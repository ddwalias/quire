#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class headerFooterRegions
{
    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    public headerFooterRegions() { }

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    private headerFooterRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    public headerFooterRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    private applicationVersionType convertedfromField;

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    public applicationVersionType convertedfrom { get => convertedfromField; set => convertedfromField = value; }

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    private bool convertedfromFieldSpecified;

    [MirrorVerified("PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests")]
    public bool convertedfromSpecified { get => convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }
}
