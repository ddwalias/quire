using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalculationOptionDefaults
{
    [MirrorVerified("PtcCalculationCoreStaticConstantsCctorsDifferentialTests")]
    static CalculationOptionDefaults() => SymbolicResultMaximumSize = 600;
}
