#nullable disable
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class ResultNodeUtils
{
    public static double ParsePlottableValue(IResultNode node)
    {
        DebugTools.DevAssert(TryParsePlottableValue(node, out var result), null);
        return result;
    }

    private static bool TryParsePlottableValue(IResultNode node, out double result)
    {
        bool flag;
        if (node is IRealResultNode)
        {
            result = (node as IRealResultNode).Value;
            flag = true;
        }
        else if (node is IComplexResultNode || node is IImaginaryResultNode)
        {
            result = double.NaN;
            flag = true;
        }
        else if (node is IUnitedValueResultNode)
        {
            flag = TryParsePlottableValue((node as IUnitedValueResultNode).Value, out result);
        }
        else
        {
            result = double.NaN;
            flag = false;
        }
        return flag;
    }
}
