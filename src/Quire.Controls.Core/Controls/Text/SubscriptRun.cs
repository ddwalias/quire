#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Text;

public partial class SubscriptRun
{
    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    public static readonly BaselineAlignment DefaultAlignment;

    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    private void InitBaseline() => base.BaselineAlignment = DefaultAlignment;

    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    protected override FontVariants FontVariantsForCopy => (FontVariants)2;
}
