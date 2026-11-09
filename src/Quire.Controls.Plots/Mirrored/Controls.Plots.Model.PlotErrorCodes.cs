#nullable disable
#pragma warning disable

using System.Globalization;

namespace Quire.Controls.Plots.Model;

public static partial class PlotErrorCodes
{
    public static string CreateTooManyPointsForTraceTypeMessage(int maximalAllowedNumberOfPoints)
    {
        return string.Format(CultureInfo.CurrentUICulture, "\t{0:N0}", TooManyPointsForPlotType, maximalAllowedNumberOfPoints);
    }
}
