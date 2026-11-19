#nullable disable

using Quire.Controls.Core.IORegions;

namespace Quire;

internal static partial class ScalarStateExtensions
{
    public static IOManagerErrorCode GetOriginalScalarState(this IScalarItemWithOriginalState item, out double originalValue, out string originalUnits)
    {
        originalValue = double.NaN;
        originalUnits = null;
        if (item == null)
        {
            return IOManagerErrorCode.AliasNotFound;
        }

        if (item.OriginalScalarState == null)
        {
            return IOManagerErrorCode.UnknownError;
        }

        originalValue = item.OriginalScalarState.Value;
        originalUnits = item.OriginalScalarState.Units;
        return item.OriginalScalarState.ErrorCode;
    }

    public static IOManagerErrorCode GetScalarState(this IScalarItem item, out double value, out string units)
    {
        value = double.NaN;
        units = null;
        if (item == null)
        {
            return IOManagerErrorCode.AliasNotFound;
        }

        if (item.ScalarState == null)
        {
            return IOManagerErrorCode.UnknownError;
        }

        value = item.ScalarState.Value;
        units = item.ScalarState.Units;
        return item.ScalarState.ErrorCode;
    }
}
