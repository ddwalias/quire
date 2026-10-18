#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class IdDefinition
{
    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public double Value { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }
}
