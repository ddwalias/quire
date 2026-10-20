#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationBuiltInVariables
{
    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private string arrayoriginField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public string arrayorigin { get => arrayoriginField; set => arrayoriginField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private double convergencetoleranceField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public double convergencetolerance { get => convergencetoleranceField; set => convergencetoleranceField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private double constrainttoleranceField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public double constrainttolerance { get => constrainttoleranceField; set => constrainttoleranceField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private string randomseedField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public string randomseed { get => randomseedField; set => randomseedField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private string prnprecisionField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public string prnprecision { get => prnprecisionField; set => prnprecisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    private string prncolwidthField;

    [MirrorVerified("PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests")]
    public string prncolwidth { get => prncolwidthField; set => prncolwidthField = value; }

    [MirrorVerified("Release IL")]
    public calculationBuiltInVariables()
    {
        arrayoriginField = "0";
        convergencetoleranceField = 0.001;
        constrainttoleranceField = 0.001;
        randomseedField = "1";
        prnprecisionField = "4";
        prncolwidthField = "8";
    }
}
