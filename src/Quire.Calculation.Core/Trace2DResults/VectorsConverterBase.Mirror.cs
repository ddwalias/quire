#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorsConverterBase
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsConverterBaseMechanicalDifferentialTests")]
    private static bool IsOfExactPlainType(PlainDataTypes vectorTypes, PlainDataTypes singleType)
    {
        if ((vectorTypes & singleType) == singleType)
        {
            return (vectorTypes & ~singleType) == 0;
        }
        return false;
    }
}
