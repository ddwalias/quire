#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceErrorCodes
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string ResultShouldBeRealVector = "PlotControl.Error.ResultShouldBeRealVector";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string QuickPlotElementsOutOfRange = "PlotControl.Error.QuickPlotElementsOutOfRange";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string EffectsShouldHaveTwoColumnsOnTheYEquation = "PlotControl.Error.EffectsShouldHaveTwoColumnsOnTheYEquation";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string BoxesResultsNotCorrect = "PlotControl.Error.BoxesResultsNotCorrect";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string ErrorsResultShouldHaveTwoColumnsOnY = "PlotControl.Error.ErrorsResultShouldHaveTwoColumnsOnY";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string NotValidTypeWhenYAxisHasLabels = "PlotControl.Error.NotValidTypeWhenYAxisHasLabels";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string UnitsAreNotSupportedInRange = "PlotControl.Error.UnitsAreNotSupportedInRange";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string ErrorTooManyPointsInResult = "PlotControl.Error.TooManyPoints";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public const string UnsupportedType = "PlotControl.Error.UnsupportedType";

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public static string CreateTooManyPointsInResultMessage(int maximalAllowedNumberOfPoints) => string.Format(CultureInfo.CurrentUICulture, "{0}\t\t\t{1:N0}", "PlotControl.Error.TooManyPoints", maximalAllowedNumberOfPoints);
}
