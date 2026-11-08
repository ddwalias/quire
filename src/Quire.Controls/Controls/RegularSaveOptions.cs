#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class RegularSaveOptions
{
    public static readonly RegularSaveOptions Empty;
    public static readonly RegularSaveOptions NonPrimeFormatForApi;
    public static readonly RegularSaveOptions WithStatesGeneration;

    static RegularSaveOptions()
    {
        Empty = new RegularSaveOptions();
        NonPrimeFormatForApi = new RegularSaveOptions
        {
            WaitForCalculationForSaveAsApi = true
        };
        WithStatesGeneration = new RegularSaveOptions
        {
            TryGeneratingInputOutputStates = true
        };
    }

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
