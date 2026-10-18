#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class CalculationDisabledException
{
    [MirrorVerified("PtcCalculationCoreCalculationDisabledExceptionMechanicalDifferentialTests")]
    public CalculationDisabledException(string message) : base(message) { }
}
