#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class RegularSaveOptions
{
    [MirrorVerified("PtcControlsRegularSaveOptionsMechanicalDifferentialTests")]
    private RegularSaveOptions() { }

    [MirrorVerified("PtcControlsRegularSaveOptionsMechanicalDifferentialTests")]
    public bool TryGeneratingInputOutputStates { get; private set; }

    [MirrorVerified("PtcControlsRegularSaveOptionsMechanicalDifferentialTests")]
    public bool IsDetachedCopy => false;

    [MirrorVerified("PtcControlsRegularSaveOptionsMechanicalDifferentialTests")]
    public bool IsHandlingException => false;

    [MirrorVerified("PtcControlsRegularSaveOptionsMechanicalDifferentialTests")]
    public bool WaitForCalculationForSaveAsApi { get; private set; }
}
