#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorsConverterBase
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsConverterBaseMechanicalDifferentialTests")]
    public static ResultValueType GetVectorsValueType(PlainDataTypes vectorType)
    {
        ResultValueType result;
        if (IsOfExactPlainType(vectorType, PlainDataTypes.Uninitialized))
        {
            result = ResultValueType.Empty;
        }
        else if ((vectorType & ~(PlainDataTypes.NaN | PlainDataTypes.Double | PlainDataTypes.UniformlyUnitedDouble | PlainDataTypes.String)) != 0)
        {
            result = ResultValueType.Empty;
        }
        else if (IsOfExactPlainType(vectorType, PlainDataTypes.NaN))
        {
            result = ResultValueType.NaNs;
        }
        else
        {
            vectorType &= ~PlainDataTypes.NaN;
            if (IsOfExactPlainType(vectorType, PlainDataTypes.Double))
            {
                result = ResultValueType.Numeric;
            }
            else if (IsOfExactPlainType(vectorType, PlainDataTypes.UniformlyUnitedDouble))
            {
                result = ResultValueType.Numeric;
            }
            else if (IsOfExactPlainType(vectorType, PlainDataTypes.String))
            {
                result = ResultValueType.String;
            }
            else
            {
                result = ResultValueType.Mixed;
            }
        }
        return result;
    }

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
