#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class ResultNodeUtils
{
    [MirrorVerified("PtcCalculationCoreResultNodeUtilsMechanicalDifferentialTests")]
    private static bool TryParsePlottableValue(IResultNode node, out double result)
    {
        bool success;
        if (node is IRealResultNode)
        {
            result = (node as IRealResultNode).Value;
            success = true;
        }
        else if (node is IComplexResultNode || node is IImaginaryResultNode)
        {
            result = double.NaN;
            success = true;
        }
        else if (node is IUnitedValueResultNode)
        {
            success = TryParsePlottableValue((node as IUnitedValueResultNode).Value, out result);
        }
        else
        {
            result = double.NaN;
            success = false;
        }

        return success;
    }
}
