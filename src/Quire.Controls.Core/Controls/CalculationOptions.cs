#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class CalculationOptions
{
    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public CalculationOptions() { }

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public bool EngineTypeAutoCorrected { get; private set; }

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public event Delegate2<string, object> CalculationOptionChanged;

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public event Delegate2<string, object> BinOctHexModeOptionChanged;
}
