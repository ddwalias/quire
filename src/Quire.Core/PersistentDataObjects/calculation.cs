#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculation
{
    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private bool cacheworksheetField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculation()
    {
        cacheworksheetField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationBuiltInVariables builtInVariablesField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationBuiltInVariables builtInVariables { get => builtInVariablesField; set => builtInVariablesField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationCalculationBehavior calculationBehaviorField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationCalculationBehavior calculationBehavior { get => calculationBehaviorField; set => calculationBehaviorField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationUnits unitsField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationUnits units { get => unitsField; set => unitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public bool cacheworksheet { get => cacheworksheetField; set => cacheworksheetField = value; }
}
