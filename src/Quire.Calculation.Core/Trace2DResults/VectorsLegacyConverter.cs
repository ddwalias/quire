#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

internal partial class VectorsLegacyConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private readonly bool _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    public bool ReadAsColumns => _readAsColumns;
}
