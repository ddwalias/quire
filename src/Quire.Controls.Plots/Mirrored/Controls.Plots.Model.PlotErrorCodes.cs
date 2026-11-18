#nullable disable
#pragma warning disable

using System.Globalization;

namespace Quire.Controls.Plots.Model;

public static partial class PlotErrorCodes
{
    public const string ErrorMinToBeSmallerThanMax = "Ptc.Plots.Axis.MIN_TO_BE_SMALLER_THAN_MAX";
    public const string ErrorMaxToBeGreaterThanMin = "Ptc.Plots.Axis.MAX_TO_BE_GREATER_THAN_MIN";
    public const string ErrorSecondTickToBeGreaterThanMin = "Ptc.Plots.Axis.SECOND_TICK_TO_BE_GREATER_THAN_MIN";
    public const string ErrorTooManyTickmarks = "Ptc.Plots.Axis.TOO_MANY_TICKMARKS";
    public const string ErrorOneLimitMustBePositive = "Ptc.Plots.Axis.ERROR_ONE_LIMIT_MUST_BE_POSITIVE";
    public const string ErrorAngularMaxToBeSmallerThan355 = "Ptc.Plots.Axis.ANGULAR_MAX_TO_BE_SMALLER_THAN_355";
    public const string ResultNotRealNumber = "RESULT_NOT_REAL_NUMBER";
    public const string MarkerOutOfRange = "Ptc.Plots.MARKER_OUT_OF_RANGE";
    public const string MarkersHasUnits = "Ptc.Plots.MARKERS_HAS_UNITS";
    public const string TickmarksHasUnits = "Ptc.Plots.TICKMARKS_HAS_UNITS";
    public const string ResultShouldBeRealVector = "PlotControl.Error.ResultShouldBeRealVector";
    public const string QuickPlotElementsOutOfRange = "PlotControl.Error.QuickPlotElementsOutOfRange";
    public const string ShouldHaveOnlyOneLabelVector = "PlotControl.Error.ShouldHaveOnlyOneLabelVector";
    public const string EffectsShouldHaveTwoColumnsOnTheYEquation = "PlotControl.Error.EffectsShouldHaveTwoColumnsOnTheYEquation";
    public const string BoxesResultsNotCorrect = "PlotControl.Error.BoxesResultsNotCorrect";
    public const string ErrorsResultShouldHaveTwoColumnsOnY = "PlotControl.Error.ErrorsResultShouldHaveTwoColumnsOnY";
    public const string NotValidTypeWhenYAxisHasLabels = "PlotControl.Error.NotValidTypeWhenYAxisHasLabels";
    public const string NotValidParameterRangeForParametric = "PlotControl.Error.NotValidParameterRangeForParametric";
    public const string ErrorTooManyPointsInResult = "PlotControl.Error.TooManyPoints";
    public const string TooManyPointsForPlotType = "PlotControl.Error.TooManyPointsForPlotType";
    public const string ResultIsNotAMatrix = "PlotControl.Error.ResultIsNotAMatrix";
    public const string OnlyNaNsAndComplex = "PlotControl.Error.OnlyNaNsAndComplex";
    public const string UnitsAreNotSupportedInRange = "PlotControl.Error.UnitsAreNotSupportedInRange";
    public const string UnsupportedType = "PlotControl.Error.UnsupportedType";

    public static bool IsLimitsCalculationRelatedCustomError(string errorCode)
    {
        return errorCode == ResultNotRealNumber || errorCode == "Ptc.Plots.TICKMARKS_HAS_UNITS";
    }

    public static string CreateTooManyPointsForTraceTypeMessage(int maximalAllowedNumberOfPoints)
    {
        return string.Format(CultureInfo.CurrentUICulture, "\t{0:N0}", TooManyPointsForPlotType, maximalAllowedNumberOfPoints);
    }
}
