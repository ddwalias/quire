using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

internal partial class VectorsLegacyConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private static bool ReadDoubleOrNone(IResultNode element, out double doubleValue)
    {
        doubleValue = double.NaN;
        if (element is IComplexResultNode || element is IImaginaryResultNode)
            return true;

        if (element is IRealResultNode real)
        {
            doubleValue = real.Value;
            return true;
        }

        return false;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private static bool ReadString(IResultNode element, out string? stringValue)
    {
        stringValue = null;
        if (element is IStringResultNode text)
        {
            stringValue = text.Value;
            return true;
        }

        return ReadDoubleOrNone(element, out double doubleValue) && double.IsNaN(doubleValue);
    }
}
