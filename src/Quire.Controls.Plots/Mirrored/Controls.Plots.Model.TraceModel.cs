#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.Model;

internal partial class TraceModel
{
    private static void CalculateRadialRange(IEnumerable<Point> points, out double min, out double max)
    {
        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        foreach (Point point in points)
        {
            if (!double.IsNaN(point.Y))
            {
                min = Math.Min(min, point.Y);
                max = Math.Max(max, point.Y);
            }
        }
    }

    private PointCollection CalculateRadialTransformByOffset(PointCollection points)
    {
        if (points == null)
        {
            return null;
        }
        double radialPointsStart = _axes[1].GetRadialPointsStart();
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            if (!point.Y.IsValid() || (_axes[1].StartExpression.IsValueValid && point.Y < radialPointsStart))
            {
                pointCollection.Add(PointExtension.NaN);
            }
            else
            {
                pointCollection.Add(new Point(point.X, point.Y - radialPointsStart));
            }
        }
        return pointCollection;
    }

    private PointCollection CalculatePolarTransformPoints(PointCollection points)
    {
        if (points == null || _axes[1] == null)
        {
            return null;
        }
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            if (point.X.IsValid() && point.Y.IsValid())
            {
                double x = point.Y * Math.Cos(point.X);
                double y = point.Y * Math.Sin(point.X);
                pointCollection.Add(new Point(x, y));
            }
            else
            {
                pointCollection.Add(PointExtension.NaN);
            }
        }
        return pointCollection;
    }

    private PointCollection CalculateLogTransformPoints(IEnumerable<Point> points)
    {
        if (points == null)
        {
            return null;
        }
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            double x = point.X;
            double y = point.Y;
            if (_axes[0].Scaling == AxisScaling.Logarithmic)
            {
                x = ((x <= 0.0) ? double.NaN : Math.Log10(x));
            }
            if (_axes[1].Scaling == AxisScaling.Logarithmic)
            {
                y = ((y <= 0.0) ? double.NaN : Math.Log10(y));
            }
            pointCollection.Add(new Point(x, y));
        }
        return pointCollection;
    }

    private bool TraceTypeIsLinearAndAxesScalingIsLinear()
    {
        return Type == TraceType.Lines;
    }

    private bool Containes(Point arg)
    {
        if (RangeAxisModel.Contains(arg.X))
        {
            return ResultAxisModel.Contains(arg.Y);
        }
        return false;
    }

    private Interval GetParameterRange()
    {
        IAxisModel rangeAxisModel = RangeAxisModel;
        if (rangeAxisModel.Type != AxesType.Angular)
        {
            return new Interval
            {
                Lower = GetXAxisLimitEditorValue(rangeAxisModel.StartExpression, rangeAxisModel.StartDefault),
                Upper = GetXAxisLimitEditorValue(rangeAxisModel.EndExpression, rangeAxisModel.EndDefault)
            };
        }
        return _angularRange;
    }

    private static double GetXAxisLimitEditorValue(IAxisLimitEditor limitEditor, double defaultValue)
    {
        if (limitEditor != null && limitEditor.IsValueValid)
        {
            return limitEditor.CalculatedValue;
        }
        return defaultValue;
    }

    private string GetxyplotScalingRl(AxesIndex axesType)
    {
        IPlotEquation equation = _equations[(int)axesType].Equation;
        return GetReplacedRl(equation.ScalingEquation, equation.ScalingRl);
    }

    private string GetExpRl(AxesIndex axesType)
    {
        return GetReplacedRl(_equations[(int)axesType].Equation.Expression);
    }

    private void InvalidateEngineResult()
    {
        EngineResult = default;
    }

    private void FireIsTraceCommitted(bool isTraceCommitted)
    {
        if (IsTraceCommittedChanged != null)
        {
            IsTraceCommittedChanged(isTraceCommitted);
        }
    }

    private void FireResultValidChanged(bool resultValid)
    {
        if (ResultValidChanged != null)
        {
            ResultValidChanged(resultValid);
        }
    }

    private void FireAnalyzedDataChanged()
    {
        Action<ITraceModel> handler = AnalyzedDataChanged;
        if (handler != null)
        {
            handler(this);
        }
    }

    private void FireValidateTraceVsPlotType()
    {
        if (ValidateTraceVsPlotType != null)
        {
            ValidateTraceVsPlotType();
        }
    }

    private void FireStyleChanged()
    {
        if (StyleChanged != null)
        {
            StyleChanged();
        }
    }

    private void SetIsCommitted(bool value)
    {
        if (value != _isCommitted)
        {
            _isCommitted = value;
            FireIsTraceCommitted(_isCommitted);
        }
    }

    protected void FireEquationError(EngineError engineError)
    {
        Action<EngineError> handler = EquationError;
        if (handler != null)
        {
            handler(engineError);
        }
    }

    protected override void OnTraceEquationSExpressionChanged()
    {
        base.OnTraceEquationSExpressionChanged();
        SetIsCommitted(value: false);
    }

    public IAxisModel GetAxis(AxesIndex axis)
    {
        return _axes[(int)axis];
    }

    public IPlotEquation GetEquation(AxesIndex axis)
    {
        return _equations[(int)axis].Equation;
    }

    public PointCollection GetPlottingPointsForDisplay()
    {
        return _plottingPointsForDisplay.ViewPoints;
    }

    public override void Commit()
    {
        FireRequestEvaluation();
    }

    public override void SetError(EngineError error)
    {
        if (error != null)
        {
            _engineResultForSerialization = null;
        }
        base.SetError(error);
    }

    protected override void PreprocessEquationSpecificError()
    {
        base.PreprocessEquationSpecificError();
        ClearPoints();
    }

    protected override void OnEquationSpecificError(ICalculatable equation)
    {
        base.OnEquationSpecificError(equation);
        FireEquationError(Error);
    }

    private void ClearPoints()
    {
        NumberOfPointsInResult = 0;
        _analyzedData.SetDefaults();
        _analyzedData.PointsCollection = new PointCollection();
        SetPlottingPoints(_analyzedData.PointsCollection);
    }

    protected override void BeforeRequestEvaluation()
    {
        SetIsCommitted(value: true);
        ResultValid = false;
        FireEquationError(null);
    }

    public AxesType GetRangeAxisType()
    {
        return GetAxis(AxesIndex.Range).Type;
    }

    private void AxisModelRangeChanged()
    {
        if (IsUsingDensityVisualization)
        {
            ImageRendererModel.BeginInit();
            ImageRendererModel.DataRange = DataRange;
            ImageRendererModel.EndInit();
        }
        _plottingPointsFilter.PlottingPoints = _analyzedData.PointsCollection != null ? new PointCollection(_analyzedData.PointsCollection) : null;
    }

    public void ResultAxisLimitValueChanged()
    {
        SetAnalyzedData();
    }

    private void UpdateFilteringAllowed()
    {
        _plottingPointsFilter.FilteringAllowed = TraceTypeIsLinearAndAxesScalingIsLinear();
    }

    private void UpdateIsTypeDefault(bool forceUpdate = false)
    {
        if (ResultValid | forceUpdate)
        {
            _isTypeDefault = (_analyzedData.HasMoreThanOneColumn && Type == TraceType.Waterfall) || (!_analyzedData.HasMoreThanOneColumn && Type == TraceType.Lines);
        }
        else
        {
            ResetIsTypeDefault();
        }
    }

    private void SetDefaultType()
    {
        if (!_isTypeDefault.HasValue)
        {
            UpdateIsTypeDefault(forceUpdate: true);
        }
        if (_isTypeDefault.HasValue && _isTypeDefault.Value)
        {
            SetTypeImpl(_analyzedData.HasMoreThanOneColumn ? TraceType.Waterfall : TraceType.Lines, callStyleChanged: true);
        }
    }

    private void AxesScalingChanged(AxisScaling scaling)
    {
        UpdateFilteringAllowed();
        SetAnalyzedData();
    }

    public void RangeAxisLimitValueChanged()
    {
        Commit();
    }

    private void EquationControlRequestEvaluation()
    {
        Commit();
    }

    public void ResetIsTypeDefault(bool? value = null)
    {
        _isTypeDefault = value;
    }

    private double? MinPlottingPointCoordinate(Func<Point, double?> selector)
    {
        if (_plottingPointsFilter.PlottingPoints == null)
        {
            return null;
        }
        return _plottingPointsFilter.PlottingPoints.Select(selector).Min();
    }

    private void SetEquationWithoutCommit(AxesIndex axis, IEquationItem equationItem)
    {
        DetachAxisEquationEventHandlers(_equations[(int)axis]);
        _equations[(int)axis] = equationItem;
        AttachAxisEquationEventHandlers(_equations[(int)axis]);
    }

    private void AttachAxisEquationEventHandlers(IEquationItem equationItem)
    {
        if (equationItem != null)
        {
            IPlotEquation equation = equationItem.Equation;
            equation.RequestEvaluation += EquationControlRequestEvaluation;
            AttachOnTraceEquationSExpressionChanged(equation);
        }
    }

    private void DetachAxisEquationEventHandlers(IEquationItem equationItem)
    {
        if (equationItem != null)
        {
            IPlotEquation equation = equationItem.Equation;
            DetachOnTraceEquationSExpressionChanged(equation);
            equation.RequestEvaluation -= EquationControlRequestEvaluation;
            ClearEquationError(equation.Expression);
            ClearEquationError(equation.ScalingEquation);
        }
    }

    private void CommitIfEquationsNotEmpty()
    {
        if (!ContainsEmptyEquation)
        {
            Commit();
        }
    }

    private void SetPlottingPoints(PointCollection value)
    {
        _plottingPointsFilter.PlottingPoints = value;
    }

    private void SetTypeImpl(TraceType type, bool callStyleChanged = false)
    {
        callStyleChanged |= _type != type;
        _type = type;
        UpdateFilteringAllowed();
        if (callStyleChanged)
        {
            FireStyleChanged();
        }
    }

    private void SetRect(double top, double left, double bottom, double right)
    {
        if (double.IsInfinity(top) || double.IsInfinity(left) || double.IsInfinity(bottom) || double.IsInfinity(right))
        {
            return;
        }
        TopLeft = new Point(left, top);
        BottomRight = new Point(right, bottom);
    }

    private void SetAnalyzedDataForAxis(IAxisModel axisModel, string[] labels, Action<IAxisModel, string[]> action)
    {
        if (labels != null)
        {
            axisModel.IsDiscrete = true;
            axisModel.MajorTickMarks.Labels = labels;
            axisModel.MajorTickMarks.Count = labels.Length;
            axisModel.ScalingChanged -= AxesScalingChanged;
            axisModel.Scaling = AxisScaling.Linear;
            axisModel.ScalingChanged += AxesScalingChanged;
        }
        else
        {
            axisModel.IsDiscrete = false;
            axisModel.MajorTickMarks.Labels = null;
            axisModel.SetTickMarksPattern(0.0, null);
            axisModel.MajorTickMarks.VisabilityPattern = null;
        }
        axisModel.XIsString = XIsDiscrete;
        axisModel.YIsString = YIsDiscrete;
        action(axisModel, labels);
    }

    private void SetAnalyzedDataForResultAxis(IAxisModel axisModel, string[] labels)
    {
        if (labels != null)
        {
            _analyzedData.RectPoints.Top = 0.0;
            _analyzedData.RectPoints.Bottom = labels.Length - 1;
        }
    }

    private void SetAnalyzedDataForRangeAxis(IAxisModel axisModel, string[] labels)
    {
        if (labels != null)
        {
            if (Type == TraceType.Effects)
            {
                axisModel.SetTickMarksPattern(-0.25, _effectsTickPattern);
                axisModel.MajorTickMarks.VisabilityPattern = new bool[3] { true, false, true };
                _analyzedData.RectPoints.Left = -0.25;
                _analyzedData.RectPoints.Right = (double)(labels.Length / 3 - 1) + 0.25;
            }
            else
            {
                axisModel.SetTickMarksPattern(0.0, null);
                axisModel.MajorTickMarks.VisabilityPattern = null;
                _analyzedData.RectPoints.Left = 0.0;
                _analyzedData.RectPoints.Right = labels.Length - 1;
            }
        }
    }

    private void SetPolarPlotRect(PointCollection points)
    {
        if (points == null)
        {
            SetRect(0.0, 0.0, double.NaN, double.NaN);
            return;
        }
        if (_axes[1].Scaling == AxisScaling.Logarithmic)
        {
            points = CalculateLogTransformPoints(points);
        }
        CalculateRadialRange(points, out double min, out double max);
        if (_axes[1].Scaling == AxisScaling.Logarithmic)
        {
            min = Math.Pow(10.0, min);
            max = Math.Pow(10.0, max);
        }
        SetRect(min, min, max, max);
    }

    public List<Point> GetGraphPointsForPlottingCoordinateOnRangeAxis(double rangeAxisValue)
    {
        if (!DebugTools.AssertNotNull(_plottingPointsForDisplay, null).Passed)
        {
            return new List<Point>();
        }
        return _plottingPointsForDisplay.GetGraphPointsForPlottingCoordinateOnRangeAxis(rangeAxisValue);
    }

    public double GetClosestPlottingCoordinateOnRangeAxis(double value, double threshold)
    {
        if (!DebugTools.AssertNotNull(_plottingPointsForDisplay, null).Passed)
        {
            return double.NaN;
        }
        return _plottingPointsForDisplay.GetClosestPlottingCoordinateOnRangeAxis(value, threshold);
    }

    private void OnPlottingPointsChanged()
    {
        UpdatePlottingPointsForDisplay();
        UpdateImageRendererModel();
        if (PlottingPointsChanged != null)
        {
            PlottingPointsChanged();
        }
    }

    private void UpdateImageRendererModel()
    {
        if (IsUsingDensityVisualization)
        {
            ImageRendererModel.BeginInit();
            ImageRendererModel.DataPoints = GetPlottingPointsForDisplay();
            ImageRendererModel.DataRange = DataRange;
            ImageRendererModel.EndInit();
        }
        else
        {
            ImageRendererModel.StopRendering();
        }
    }

    public bool IsUsingDensityVisualization
    {
        get
        {
            if (AreDensityPointsSupported)
            {
                if (!ForcedToDensityVisualization)
                {
                    if (RestrictedToDensityPointsOrSolidLine)
                    {
                        return LineStyle == LineStyle.None;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public bool ResultValid
    {
        get
        {
            return _resultValid;
        }
        set
        {
            _resultValid = value;
            FireResultValidChanged(_resultValid);
        }
    }

    public bool IsTypeDefault
    {
        get
        {
            if (_isTypeDefault.HasValue)
            {
                return _isTypeDefault.Value;
            }
            return false;
        }
    }

    public bool ContainsEmptyEquation
    {
        get
        {
            if (!GetEquation(AxesIndex.Range).IsPlotEquationEmpty)
            {
                return GetEquation(AxesIndex.Result).IsPlotEquationEmpty;
            }
            return true;
        }
    }

    public Point TopLeft { get; private set; }

    public Point BottomRight { get; private set; }

    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;
            ImageRendererModel.StyleOwner.Color = _color;
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return _lineStyle;
        }
        set
        {
            if (value == _lineStyle)
            {
                return;
            }
            bool isUsingDensityVisualization = IsUsingDensityVisualization;
            _lineStyle = value;
            bool isUsingDensityVisualization2 = IsUsingDensityVisualization;
            if (isUsingDensityVisualization != isUsingDensityVisualization2)
            {
                UpdateImageRendererModel();
            }
        }
    }

    public TraceType Type
    {
        get
        {
            return _type;
        }
        set
        {
            if (_type != value)
            {
                SetTypeImpl(value);
                UpdateIsTypeDefault();
                ClearPoints();
                InvalidateEngineResult();
                Commit();
            }
        }
    }

    private int PlottingPointsCount
    {
        get
        {
            if (_plottingPointsFilter == null || _plottingPointsFilter.FilteredPointsCollection == null)
            {
                return 0;
            }
            return _plottingPointsFilter.FilteredPointsCollection.Count;
        }
    }

    private int FinalNumberOfDataPointsToRender
    {
        get
        {
            if (_plottingPointsFilter == null || _plottingPointsFilter.FilteredPointsCollection == null)
            {
                return 0;
            }
            return _plottingPointsFilter.NotNullPointsCount;
        }
    }

    public bool ForcedToDensityVisualization
    {
        get
        {
            if (!ImageRendererModel.HasCachedBitmap)
            {
                return FinalNumberOfDataPointsToRender >= ControlSettings.Instance.MinPointsForForceDensity;
            }
            return true;
        }
    }

    public EngineResult EngineResultForSerialization
    {
        get
        {
            EngineResult engineResult = _engineResultForSerialization ?? EngineResult;
            if (!ShouldNotCopyFullResults)
            {
                return engineResult;
            }
            return CopyMinMaxAndUnitsResults(engineResult);
        }
        private set
        {
            _engineResultForSerialization = value;
        }
    }

    private IAxisModel RangeAxisModel => _axes[0];

    private IAxisModel ResultAxisModel => _axes[1];

    private int NumberOfPointsInResult { get; set; }

    public override bool IsCommitted => _isCommitted;

    public bool ShouldNotCopyFullResults { get; set; }

    public bool RestrictedToDensityPointsOrSolidLine
    {
        get
        {
            int finalNumberOfDataPointsToRender = FinalNumberOfDataPointsToRender;
            if (ControlSettings.Instance.MinPointsForDensity <= finalNumberOfDataPointsToRender)
            {
                return finalNumberOfDataPointsToRender < ControlSettings.Instance.MinPointsForForceDensity;
            }
            return false;
        }
    }

    public bool AreDensityPointsSupported => PlotType != PlotType.Polar;

    public ITraceImageRendererModel ImageRendererModel { get; private set; }

    public PlotType PlotType { get; private set; }

    public TraceSymbol Symbol { get; set; }

    public double SymbolWeight { get; set; }

    public double SymbolFrequency { get; set; }

    public double LineWeight { get; set; }

    public string Label { get; set; }

    public bool XIsDiscrete { get; private set; }

    public bool YIsDiscrete { get; private set; }

    private void AnalyzeAxisAndTypeData(ITrace2DResultNode plotResult)
    {
        _analyzedData.RectPoints.Left = plotResult.RangeAxisInfo.Min;
        _analyzedData.RectPoints.Right = plotResult.RangeAxisInfo.Max;
        _analyzedData.RangeUnits = plotResult.RangeAxisInfo.Units;
        _analyzedData.RectPoints.Top = plotResult.ResultAxisInfo.Min;
        _analyzedData.RectPoints.Bottom = plotResult.ResultAxisInfo.Max;
        _analyzedData.ResultUnits = plotResult.ResultAxisInfo.Units;
        IsNumOfPointsMutable = plotResult.TraceResultType == Trace2DResultType.Parametric;
        _equations[0].Equation.Units = _analyzedData.RangeUnits;
        _equations[1].Equation.Units = _analyzedData.ResultUnits;
    }

    private void SetAnalyzedData()
    {
        SetPlottingPoints(_analyzedData.PointsCollection == null ? null : new PointCollection(_analyzedData.PointsCollection));
        SetAnalyzedDataForAxis(GetAxis(AxesIndex.Result), _analyzedData.YLabels, SetAnalyzedDataForResultAxis);
        SetAnalyzedDataForAxis(GetAxis(AxesIndex.Range), _analyzedData.XLabels, SetAnalyzedDataForRangeAxis);
        if (PlotType.Polar == PlotType)
        {
            SetPolarPlotRect(_analyzedData.PointsCollection);
        }
        else
        {
            SetRect(_analyzedData.RectPoints.Top, _analyzedData.RectPoints.Left, _analyzedData.RectPoints.Bottom, _analyzedData.RectPoints.Right);
        }
        FireAnalyzedDataChanged();
        ResultValid = true;
    }

    private bool CreateAllPointsByTraceType(IVectorListResultNode xResult, IVectorListResultNode yResult)
    {
        IList<IList<double>> list = TraceResultAnalyzer.ToDoubleDataList(xResult);
        IList<IList<double>> list2 = TraceResultAnalyzer.ToDoubleDataList(yResult);
        IList<Point> pointsCollection = _analyzedData.PointsCollection;
        if (list.IsNullOrEmpty() || list2.IsNullOrEmpty())
        {
            return false;
        }
        bool result = false;
        switch (Type)
        {
            case TraceType.Lines:
            case TraceType.Columns:
            case TraceType.Bars:
            case TraceType.Stems:
                result = TraceResultAnalyzer.AddOneDataSegment(pointsCollection, list.First(), list2.First(), ensureTrailingNaN: false);
                break;
            case TraceType.Waterfall:
                result = TraceResultAnalyzer.BuildPointsForWaterfall(pointsCollection, list, list2);
                break;
            case TraceType.Boxes:
                result = TraceResultAnalyzer.BuildPointsForBoxes(pointsCollection, list.First(), list2);
                break;
            case TraceType.Errors:
                result = TraceResultAnalyzer.BuildTuplesOfPoints(pointsCollection, list.First(), list2.Take(2).ToArray());
                break;
            case TraceType.Effects:
                result = TraceResultAnalyzer.BuildTuplesOfPoints(pointsCollection, list.First(), list2.Take(2).ToArray(), new double[2] { -0.25, 0.25 });
                break;
        }
        return result;
    }

    private bool AnalyzeResult(IMatrixResultNode matrix)
    {
        string errorMessageCode;
        ITrace2DResultNode plotResult = _converter.ConvertToVectorsForm(matrix, out errorMessageCode);
        if (errorMessageCode.IsNotEmpty())
        {
            throw new PlotInvalidResultException(errorMessageCode);
        }
        AnalyzeVectors(plotResult);
        return true;
    }

    private bool AnalyzeResult(ITrace2DResultNode traceReducedResult)
    {
        AnalyzeVectors(traceReducedResult);
        return true;
    }

    private void AnalyzeVectors(ITrace2DResultNode plotResult)
    {
        PlotInvalidResultException ex = null;
        try
        {
            if (plotResult.TraceResultType == Trace2DResultType.Parametric)
            {
                Interval parameterRange = GetParameterRange();
                if (parameterRange.Upper < parameterRange.Lower)
                {
                    ClearPoints();
                    EngineResultForSerialization = new EngineResult(null, new EngineErrorCollection
                    {
                        new EngineError
                        {
                            Message = "PlotControl.Error.NotValidParameterRangeForParametric"
                        }
                    }, EngineResult.SynchronizationStatus);
                    ex = new PlotInvalidResultException("PlotControl.Error.NotValidParameterRangeForParametric");
                }
            }
            if (ex == null && (!plotResult.DataPoints.IsEmpty || !ImageRendererModel.HasCachedBitmap))
            {
                IVectorListResultNode rangePoints = plotResult.DataPoints.RangePoints;
                IVectorListResultNode resultPoints = plotResult.DataPoints.ResultPoints;
                if (NumberOfPointsInResult == 0)
                {
                    NumberOfPointsInResult = Math.Max(rangePoints.ElementsCount, resultPoints.ElementsCount);
                }
                if (!AnalyzeResultData(rangePoints, resultPoints))
                {
                    ex = new PlotInvalidResultException("PlotControl.Error.OnlyNaNsAndComplex");
                }
            }
            AnalyzeAxisAndTypeData(plotResult);
        }
        catch (Exception ex2)
        {
            DebugTools.FailAssertion(ex2.Message);
            ex = new PlotInvalidResultException("PlotControl.Error.ResultIsNotAMatrix", ex2);
        }
        if (ex == null)
        {
            EngineResultForSerialization = new EngineResult(plotResult, null, EngineResult.SynchronizationStatus);
            return;
        }
        throw ex;
    }
}
