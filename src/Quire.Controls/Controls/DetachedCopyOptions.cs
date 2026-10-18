#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class DetachedCopyOptions
{
    [MirrorVerified("PtcControlsDetachedCopyOptionsMechanicalDifferentialTests")]
    private DetachedCopyOptions() { }

    [MirrorVerified("PtcControlsDetachedCopyOptionsMechanicalDifferentialTests")]
    public bool TryGeneratingInputOutputStates => false;

    [MirrorVerified("PtcControlsDetachedCopyOptionsMechanicalDifferentialTests")]
    public bool IsDetachedCopy => true;

    [MirrorVerified("PtcControlsDetachedCopyOptionsMechanicalDifferentialTests")]
    public bool IsHandlingException { get; private set; }

    [MirrorVerified("PtcControlsDetachedCopyOptionsMechanicalDifferentialTests")]
    public bool WaitForCalculationForSaveAsApi => false;
}
