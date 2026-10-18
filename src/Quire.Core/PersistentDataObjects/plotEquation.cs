#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class plotEquation
{
    [MirrorVerified("PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests")]
    public plotEquation() { }

    [MirrorVerified("PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests")]
    private mathType[] mathField;

    [MirrorVerified("PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests")]
    public mathType[] math { get => mathField; set => mathField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests")]
    private mathFormattingType formattingOverrideField;

    [MirrorVerified("PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests")]
    public mathFormattingType formattingOverride { get => formattingOverrideField; set => formattingOverrideField = value; }
}
