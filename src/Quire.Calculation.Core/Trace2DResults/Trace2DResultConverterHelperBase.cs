#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public abstract partial class Trace2DResultConverterHelperBase
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultConverterHelperBaseMechanicalDifferentialTests")]
    protected ITrace2DResultOwner ResultsOwner { get; private set; }

    public abstract bool DiscreteValuesAllowed { get; }

    public abstract bool RangeUnitsAllowed { get; }

    public TraceType CurrentType => ResultsOwner.Type;

    public abstract int MaxNumberOf2DPoints { get; }

    public bool IsTypeDefault => ResultsOwner.IsTypeDefault;

    protected Trace2DResultConverterHelperBase(ITrace2DResultOwner resultsOwner)
    {
        ResultsOwner = resultsOwner;
    }

    public string AssertValuesTypeMatchTraceType(bool xIsDiscrete, bool yIsDiscrete, TraceType traceTypePreset)
    {
        string result = null;
        if ((xIsDiscrete | yIsDiscrete) && !DiscreteValuesAllowed)
        {
            result = "PlotControl.Error.ResultShouldBeRealVector";
        }
        else if (yIsDiscrete && traceTypePreset != TraceType.Bars && traceTypePreset != TraceType.Lines)
        {
            result = "PlotControl.Error.NotValidTypeWhenYAxisHasLabels";
        }
        else if (!xIsDiscrete && traceTypePreset == TraceType.Effects)
        {
            result = "PlotControl.Error.EffectsShouldHaveTwoColumnsOnTheYEquation";
        }
        return result;
    }

    public string ValidateColumnsAndUnits(int numberOfPointsInResult, int yResultColumns, IList<Unit> rangeUnits, TraceType traceType)
    {
        string text = null;
        if (rangeUnits.IsNotEmpty() && !RangeUnitsAllowed)
        {
            text = "PlotControl.Error.UnitsAreNotSupportedInRange";
        }
        if (text.IsNullOrEmpty())
        {
            text = ValidateNumberOfPointsLimit(numberOfPointsInResult, traceType);
        }
        if (text.IsNullOrEmpty())
        {
            text = ValidateEnoughResultColumnsForTraceType(yResultColumns, traceType);
        }
        return text;
    }

    public virtual string ValidateNumberOfPointsLimit(int numberOfPointsInResult, TraceType traceType)
    {
        int maxNumberOf2DPoints = MaxNumberOf2DPoints;
        if (numberOfPointsInResult <= maxNumberOf2DPoints)
        {
            return null;
        }
        return TraceErrorCodes.CreateTooManyPointsInResultMessage(maxNumberOf2DPoints + 1);
    }

    private static string ValidateEnoughResultColumnsForTraceType(int numberOfResultColumns, TraceType traceType)
    {
        string result = null;
        if (numberOfResultColumns >= TraceTypeDefinitions.GetMinResultColumns(traceType))
        {
            return result;
        }
        switch (traceType)
        {
            case TraceType.Effects:
                result = "PlotControl.Error.EffectsShouldHaveTwoColumnsOnTheYEquation";
                break;
            case TraceType.Boxes:
                result = "PlotControl.Error.BoxesResultsNotCorrect";
                break;
            case TraceType.Errors:
                result = "PlotControl.Error.ErrorsResultShouldHaveTwoColumnsOnY";
                break;
            default:
                DebugTools.FailAssertion(null, Array.Empty<object>());
                result = "PlotControl.Error.UnsupportedType";
                break;
        }
        return result;
    }
}
