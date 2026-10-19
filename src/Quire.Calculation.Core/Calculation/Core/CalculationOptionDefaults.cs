using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalculationOptionDefaults
{
    [MirrorVerified("PtcCalculationCoreStaticConstantsCctorsMechanicalDifferentialTests")]
    static CalculationOptionDefaults() => SymbolicResultMaximumSize = 600;
}
