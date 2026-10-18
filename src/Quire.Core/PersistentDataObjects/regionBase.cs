#nullable disable
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentDataObjects;

public partial class regionBase
{
    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private string regionidField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public string regionid { get => regionidField; set => regionidField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private string tagField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public string Tag { get => tagField; set => tagField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool mc14conversionlossField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool mc14conversionloss { get => mc14conversionlossField; set => mc14conversionlossField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private applicationVersionType convertedfromField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public applicationVersionType convertedfrom { get => convertedfromField; set => convertedfromField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool convertedfromFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool convertedfromSpecified { get => convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double heightField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double height { get => heightField; set => heightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool heightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool heightSpecified { get => heightFieldSpecified; set => heightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double widthField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double width { get => widthField; set => widthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool widthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool widthSpecified { get => widthFieldSpecified; set => widthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double actualWidthField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double actualWidth { get => actualWidthField; set => actualWidthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool actualWidthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool actualWidthSpecified { get => actualWidthFieldSpecified; set => actualWidthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double actualHeightField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double actualHeight { get => actualHeightField; set => actualHeightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool actualHeightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool actualHeightSpecified { get => actualHeightFieldSpecified; set => actualHeightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double alignxField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double alignx { get => alignxField; set => alignxField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool alignxFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool alignxSpecified { get => alignxFieldSpecified; set => alignxFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double alignyField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double aligny { get => alignyField; set => alignyField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool alignyFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool alignySpecified { get => alignyFieldSpecified; set => alignyFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool showBorderField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool ShowBorder { get => showBorderField; set => showBorderField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler { get; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public SolveBlockMemberType SolveBlockMemberType { get; set; }
}
