#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalculationOptionDefaults
{
    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool StrictComparisions = true;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool StringsIndexedFromOrigin = false;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool StrictSingularityChecking = false;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool UnitsAndConstantsInSymbolics = false;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const int NumberOfWorkerThreads = 1;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool RestrictedMode = false;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public static int SymbolicResultMaximumSize = 600;

    [MirrorVerified("PtcCalculationCoreCalculationOptionDefaultsMechanicalDifferentialTests")]
    public const bool BinOctHexMode = false;
}
