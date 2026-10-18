#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class MatrixDefinition
{
    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public double[,] Value { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }
}
