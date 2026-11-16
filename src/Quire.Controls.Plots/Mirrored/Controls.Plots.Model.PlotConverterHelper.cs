using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class PlotConverterHelper : Trace2DResultConverterHelperBase
{
    private readonly TraceModel _plotTraceOwner;

    public override bool DiscreteValuesAllowed => _plotTraceOwner.PlotType != PlotType.Polar;

    public override bool RangeUnitsAllowed => _plotTraceOwner.GetRangeAxisType() != AxesType.Angular;

    public override int MaxNumberOf2DPoints
    {
        get
        {
            if (!_plotTraceOwner.AreDensityPointsSupported)
            {
                return ControlSettings.Instance.MinPointsForForceDensity;
            }
            return ControlSettings.Instance.MaxNumberOf2dPoints;
        }
    }

    public PlotConverterHelper(TraceModel resultsOwner)
        : base(resultsOwner)
    {
        _plotTraceOwner = resultsOwner;
    }

    public override string ValidateNumberOfPointsLimit(int numberOfPointsInResult, TraceType traceType)
    {
        int maxNumberOf2DPoints = MaxNumberOf2DPoints;
        int minPointsForForceDensity = ControlSettings.Instance.MinPointsForForceDensity;
        int minPointsForDensity = ControlSettings.Instance.MinPointsForDensity;
        DebugTools.DevAssert(maxNumberOf2DPoints >= minPointsForForceDensity, null);
        DebugTools.DevAssert(minPointsForForceDensity >= minPointsForDensity, null);
        string text = base.ValidateNumberOfPointsLimit(numberOfPointsInResult, traceType);
        if (text.IsNotEmpty())
        {
            return text;
        }
        switch (traceType)
        {
        case TraceType.Waterfall:
            if (numberOfPointsInResult >= minPointsForForceDensity)
            {
                text = PlotErrorCodes.CreateTooManyPointsForTraceTypeMessage(minPointsForForceDensity);
            }
            break;
        default:
            if (numberOfPointsInResult >= minPointsForDensity)
            {
                text = PlotErrorCodes.CreateTooManyPointsForTraceTypeMessage(minPointsForDensity);
            }
            break;
        case TraceType.Lines:
            break;
        }
        return text;
    }
}
