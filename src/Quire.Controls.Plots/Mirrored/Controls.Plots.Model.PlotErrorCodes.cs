#nullable disable
#pragma warning disable

using System.Globalization;

namespace Quire.Controls.Plots.Model;

public static partial class PlotErrorCodes
{
    public static bool IsLimitsCalculationRelatedCustomError(string errorCode)
    {
        return errorCode == ResultNotRealNumber || errorCode == "Ptc.Plots.TICKMARKS_HAS_UNITS";
    }

    public static string CreateTooManyPointsForTraceTypeMessage(int maximalAllowedNumberOfPoints)
    {
        return string.Format(CultureInfo.CurrentUICulture, "\t{0:N0}", TooManyPointsForPlotType, maximalAllowedNumberOfPoints);
    }
}
