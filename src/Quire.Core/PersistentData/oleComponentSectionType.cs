#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class oleComponentSectionType
{
    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    private OleComponentSectionVisibility visibilityField;

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    public oleComponentSectionType()
    {
        visibilityField = OleComponentSectionVisibility.Visible;
    }

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    public OleComponentSectionVisibility Visibility { get => visibilityField; set => visibilityField = value; }

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    private double heightField;

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    public double Height { get => heightField; set => heightField = value; }

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    private bool heightFieldSpecified;

    [MirrorVerified("PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests")]
    public bool HeightSpecified { get => heightFieldSpecified; set => heightFieldSpecified = value; }
}
