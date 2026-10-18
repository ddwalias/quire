#nullable disable
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class SexprDefinition
{
    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override void SetResult(IResultNode result) { }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override bool IsErrorOnScalingUnits() => false;

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }
}
