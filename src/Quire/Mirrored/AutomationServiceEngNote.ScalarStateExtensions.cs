namespace Quire.AutomationServiceEngNote;

internal static partial class ScalarStateExtensions
{
    public static global::Quire.Controls.Core.IORegions.IOManagerErrorCode GetOriginalScalarState(this global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState item, out double originalValue, out string originalUnits)
    {
        originalValue = double.NaN;
        originalUnits = null;
        if (item == null)
        {
            return (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)1;
        }

        if (item.OriginalScalarState == null)
        {
            return (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)(-1);
        }

        originalValue = item.OriginalScalarState.Value;
        originalUnits = item.OriginalScalarState.Units;
        return item.OriginalScalarState.ErrorCode;
    }

    public static global::Quire.Controls.Core.IORegions.IOManagerErrorCode GetScalarState(this global::Quire.Controls.Core.IORegions.IScalarItem item, out double value, out string units)
    {
        value = double.NaN;
        units = null;
        if (item == null)
        {
            return (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)1;
        }

        if (item.ScalarState == null)
        {
            return (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)(-1);
        }

        value = item.ScalarState.Value;
        units = item.ScalarState.Units;
        return item.ScalarState.ErrorCode;
    }
}
