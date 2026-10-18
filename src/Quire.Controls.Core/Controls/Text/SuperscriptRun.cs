#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Text;

public partial class SuperscriptRun
{
    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    public static readonly BaselineAlignment DefaultAlignment;

    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    private void InitBaseline() => base.BaselineAlignment = DefaultAlignment;

    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    protected override FontVariants FontVariantsForCopy => (FontVariants)1;
}
