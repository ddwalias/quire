#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NodeMessageAdorner
{
    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    protected override void AddToAdornerLayer() => FireAdornerAdded();

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private string _message;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public string Message => _message;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private readonly NodeMessageAdornerType _type;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public NodeMessageAdornerType Type => _type;
}
