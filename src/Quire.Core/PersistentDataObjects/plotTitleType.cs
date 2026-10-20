#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class plotTitleType
{
    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    private textType titleField;

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    public textType title { get => titleField; set => titleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    private string titlepositionField;

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    public string titleposition { get => titlepositionField; set => titlepositionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    private bool showborderField;

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    public bool showborder { get => showborderField; set => showborderField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    private bool isverticalField;

    [MirrorVerified("PtcPersistentDataObjectsplotTitleTypeMechanicalDifferentialTests")]
    public bool isvertical { get => isverticalField; set => isverticalField = value; }

    [MirrorVerified("Release IL")]
    public plotTitleType()
    {
        displayField = false;
        titlepositionField = "PlotBoundaryTop";
        isverticalField = false;
    }
}
