#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MarginLineAdorner
{
    [MirrorVerified("PtcControlsCoreMarginLineAdornerMechanicalDifferentialTests")]
    protected override void AddToAdornerLayer() => FireAdornerAdded();

    [MirrorVerified("PtcControlsCoreMarginLineAdornerMechanicalDifferentialTests")]
    public bool IsDisplayed { get; private set; }
}
