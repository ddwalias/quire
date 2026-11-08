#nullable disable
#pragma warning disable

namespace Quire.Controls.ExcelComponent.Model;

public static partial class ChartComponentErrors
{
    public const string ComExceptionError = "chart_com_exception";
    public const string LimitsTooClose = "chart_limits_too_close";
    public const string MinLimitToBeSmallerThanMax = "chart_min_limit_to_be_smaller_than_max";
    public const string MaxLimitToBeGreaterThanMin = "chart_max_limit_to_be_larger_than_min";
    public const string LimitResultNotRealNumber = "chart_limit_result_not_real_number";
    public const string LimitHasUnits = "chart_limit_has_units";
    public const string DuplicateTraceExpression = "chart_duplicate_trace_expression";
    public const string DuplicateLimitExpression = "chart_duplicate_limit_expression";
    public const string InvalidIdentifier = "chart_invalid_identifier";
    public const string InvalidIndex = "chart_invalid_index";
    public const string OnlyNaNsAndComplex = "chart_only_nans_and_complex";
    public static string ResultShouldBeRealVector = AddChartPrefix("PlotControl.Error.ResultShouldBeRealVector");
    public static string PlotElementsOutOfRange = AddChartPrefix("PlotControl.Error.QuickPlotElementsOutOfRange");
    public static string TooManyPointsInResult = AddChartPrefix("PlotControl.Error.TooManyPoints");

    public static string AddChartPrefix(string err)
    {
        return "chart_" + err;
    }
}
