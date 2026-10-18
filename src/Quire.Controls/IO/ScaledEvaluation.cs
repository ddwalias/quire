#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class ScaledEvaluation
{
    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public override bool IsErrorOnScalingUnits() => false;

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public string BaseSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }
}
