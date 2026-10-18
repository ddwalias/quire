#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

public partial class CalculationVariables
{
    [MirrorVerified("PtcControlsCalculationVariablesMechanicalDifferentialTests")]
    public event Delegate2<string, object> CalculationVariableChanged;
}
