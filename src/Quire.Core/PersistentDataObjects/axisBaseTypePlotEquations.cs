#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisBaseTypePlotEquations
{
    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests")]
    public axisBaseTypePlotEquations()
    {
        displayField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests")]
    private plotEquation[] plotEquationField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests")]
    public plotEquation[] plotEquation { get => plotEquationField; set => plotEquationField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }
}
