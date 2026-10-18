#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorsLazyMatrixConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly bool _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    public bool ReadAsColumns => _readAsColumns;
}
