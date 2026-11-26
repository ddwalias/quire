#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Media3D;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.Model;

public partial class PlotModel3D
{
    private const string EquationIndexValueShouldBeValid = "Equation index Value should be valid";

    private const string EquationListShouldBeNonEmpty = "Equation list should be non empty";

    private const string DirectionShouldBeValid = "Direction should be valid";

    private readonly global::System.Collections.Generic.IList<global::Quire.Controls.Plots.Plot3D.Model.IPlotTraceModel3D> _traces;

    private readonly global::Quire.Controls.Plots.Plot3D.Model.IEquationListModel3D _equationListModel3D;

    private global::Quire.Controls.Plots.Presenter.IAxisModel _xAxis;

    private global::Quire.Controls.Plots.Presenter.IAxisModel _yAxis;

    private global::Quire.Controls.Plots.Presenter.IAxisModel _zAxis;

    public event global::System.Action<global::Quire.Controls.Plots.Plot3D.Model.IPlotTraceModel3D, int> TraceInserted;

    public event global::System.Action<global::Quire.Controls.Plots.Plot3D.Model.IPlotTraceModel3D> TraceRemoved;

    public event global::System.Action<int, int, global::Quire.Controls.Plots.Plot3D.Model.IPlotEquationModel3D> EquationAdded;

    public event global::System.Action<int, int, global::Quire.Direction> EquationRemoved;

    internal IEnumerable<IPlotEquationModel3D> Equations => _equationListModel3D;

    internal IEnumerable<IPlotTraceModel3D> Traces => _traces;

    public override global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> GetHiddenAndVisibleCalculatables() => base.GetHiddenAndVisibleCalculatables();

    private static bool AssertThatAxisAndAxisCalculatableIsNotNull(IAxisModel axisModel)
    {
        if (DebugTools.AssertNotNull(axisModel, null).Passed)
        {
            return DebugTools.AssertNotNull(axisModel.AxisCalculatables, null).Passed;
        }
        return false;
    }

    private double _xStart;

    private double _xEnd;

    private double _yStart;

    private double _yEnd;

    private double _zStart;

    private double _zEnd;

    private global::Quire.Calculation.Core.SynchronizationStatus _synchronizationStatus;

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            return _synchronizationStatus;
        }
        private set
        {
            if (_synchronizationStatus != value)
            {
                _synchronizationStatus = value;
                FirePropertyChanged("SynchronizationStatus");
            }
        }
    }

    private void AttachAxisEventHandlers(IAxisModel axisModel)
    {
        if (axisModel != null)
        {
            axisModel.Changed += OnChanged;
            axisModel.PropertyChanged += AxisPropertyChanged;
            axisModel.RangeChanged += AxisRangeChanged;
            AttachOrDetachLimitEditorEventHandlers(axisModel, null, attach: true);
        }
    }

    private void DetachAxisEventHandlers(IAxisModel axisModel)
    {
        if (axisModel != null)
        {
            axisModel.Changed -= OnChanged;
            axisModel.PropertyChanged -= AxisPropertyChanged;
            axisModel.RangeChanged -= AxisRangeChanged;
            AttachOrDetachLimitEditorEventHandlers(axisModel, null, attach: false);
        }
    }

    private void AxisRangeChanged()
    {
        SetRangeOnTraces(GetAxesRange());
    }

    private void AttachOrDetachLimitEditorEventHandlers(global::Quire.Controls.Plots.Presenter.IAxisModel axisModel, string propertyName, bool attach) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void AxisPropertyChanged(object sender, PropertyChangedEventArgs eArgs)
    {
        IAxisModel axisModel = sender as IAxisModel;
        switch (eArgs.PropertyName)
        {
            case "StartExpression":
            case "SecondTickExpression":
            case "EndExpression":
                AttachOrDetachLimitEditorEventHandlers(axisModel, eArgs.PropertyName, attach: true);
                break;
        }
    }

    private void UpdateLimitsAndBounds() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static double AxisLimitEditorCalculatedValue(IAxisLimitEditor axisLimitEditor)
    {
        if (!HasValidUserDefinedValue(axisLimitEditor))
        {
            return double.NaN;
        }
        return axisLimitEditor.CalculatedValue;
    }

    private void UpdateCommonTraceBounds()
    {
        Rect3D commonTraceBounds = GetCommonTraceBounds();
        Rect3D commonRange = CalculateInitialCommonRange(commonTraceBounds);
        IAxisModel[] axes = new IAxisModel[] { _xAxis, _yAxis, _zAxis };
        axes.Each(axis => axis.RangeChanged -= AxisRangeChanged);
        try
        {
            SetRangeOnAxes(commonRange);
        }
        finally
        {
            axes.Each(axis => axis.RangeChanged += AxisRangeChanged);
        }
        AxisRangeChanged();
    }

    private void AxisExpressionHasErrorsChanged()
    {
        SetAxesHaveMinMaxErrors(AxisHasMinMaxErrors(XAxis) || AxisHasMinMaxErrors(YAxis) || AxisHasMinMaxErrors(ZAxis));
    }

    private static bool AxisHasMinMaxErrors(IAxisModel axis)
    {
        if (!axis.StartExpression.HasErrors)
        {
            return axis.EndExpression.HasErrors;
        }
        return true;
    }

    private void SetAxesHaveMinMaxErrors(bool axesHaveErrors)
    {
        _traces.Each(trace => trace.AxesHaveMinMaxErrors = axesHaveErrors);
    }

    public IAxisModel XAxis
    {
        get
        {
            return _xAxis;
        }
        set
        {
            DetachAxisEventHandlers(_xAxis);
            _xAxis = value;
            AttachAxisEventHandlers(_xAxis);
        }
    }

    public IAxisModel YAxis
    {
        get
        {
            return _yAxis;
        }
        set
        {
            DetachAxisEventHandlers(_yAxis);
            _yAxis = value;
            AttachAxisEventHandlers(_yAxis);
        }
    }

    public IAxisModel ZAxis
    {
        get
        {
            return _zAxis;
        }
        set
        {
            DetachAxisEventHandlers(_zAxis);
            _zAxis = value;
            AttachAxisEventHandlers(_zAxis);
        }
    }

    public Transform3D CameraTransform
    {
        get
        {
            return _cameraTransform;
        }
        set
        {
            _cameraTransform = value;
            OnPropertyChanged("CameraTransform");
        }
    }

    private global::System.Windows.Media.Media3D.Transform3D _cameraTransform;

    private global::Quire.Controls.Core.CameraType3D _plotCameraType;

    public CameraType3D PlotCameraType
    {
        get
        {
            return _plotCameraType;
        }
        set
        {
            if (value != PlotCameraType)
            {
                _plotCameraType = value;
                OnPropertyChanged("PlotCameraType");
            }
        }
    }

    private void TracePlotCalculatableError(object sender, EngineErrorEventArgs e)
    {
        OnPlotCalculatableError(e);
    }

    private void RemoveTrace(IPlotTraceModel3D trace)
    {
        trace.ResetOnRemoval();
        trace.PropertyChanged -= TracePropertyChanged;
        trace.Changed -= OnChanged;
        trace.PlotCalculatableError -= TracePlotCalculatableError;
        DebugTools.DevAssert(_traces.Remove(trace), null);
        FireTraceRemoved(trace);
    }

    private void ComputeTotalTracesSynchronizationStatus()
    {
        SynchronizationStatus = _traces.Any(trace => trace.SynchronizationStatus == SynchronizationStatus.Pending) ? SynchronizationStatus.Pending : SynchronizationStatus.Synchronized;
    }

    private void TracePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)
    {
        IPlotTraceModel3D trace = sender as IPlotTraceModel3D;
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(trace != null, null, Array.Empty<object>()))
        {
            return;
        }
        switch (args.PropertyName)
        {
            case "TraceBounds":
                UpdateCommonTraceBounds();
                break;
            case "CalculatingStatus":
                ComputeTotalTracesCalculationStatus();
                break;
            case "SynchronizationStatus":
                ComputeTotalTracesSynchronizationStatus();
                break;
            case "Error":
                HasError = _traces.Any(traceModel => traceModel.Error != null);
                break;
        }
    }

    private void SetRangeOnAxes(Rect3D newRange)
    {
        SetRangeOnAxis(newRange.X, newRange.SizeX, XAxis);
        SetRangeOnAxis(newRange.Y, newRange.SizeY, YAxis);
        SetRangeOnAxis(newRange.Z, newRange.SizeZ, ZAxis);
    }

    private static void SetRangeOnAxis(double start, double size, IAxisModel axisModel)
    {
        double value = start + size;
        axisModel.SetRange(start, value);
    }

    private Rect3D GetCommonTraceBounds()
    {
        Rect3D result = Rect3D.Empty;
        foreach (IPlotTraceModel3D trace in _traces.Where(traceModel => traceModel.TraceBounds != Rect3D.Empty))
        {
            result.Union(trace.TraceBounds);
        }
        return result;
    }

    private void SetRangeOnTraces(Rect3D newRange)
    {
        foreach (IPlotTraceModel3D trace in _traces)
        {
            trace.CommonRange = newRange;
        }
    }

    protected Rect3D CalculateInitialCommonRange(Rect3D commonTraceBounds)
    {
        Interval commonInterval = GetCommonInterval(_xAxis, commonTraceBounds.X, commonTraceBounds.X + commonTraceBounds.SizeX);
        Interval commonInterval2 = GetCommonInterval(_yAxis, commonTraceBounds.Y, commonTraceBounds.Y + commonTraceBounds.SizeY);
        Interval commonInterval3 = GetCommonInterval(_zAxis, commonTraceBounds.Z, commonTraceBounds.Z + commonTraceBounds.SizeZ);
        return new Rect3D
        {
            X = commonInterval.Lower,
            SizeX = commonInterval.Width,
            Y = commonInterval2.Lower,
            SizeY = commonInterval2.Width,
            Z = commonInterval3.Lower,
            SizeZ = commonInterval3.Width
        };
    }

    private Rect3D GetAxesRange()
    {
        return new Rect3D
        {
            X = _xAxis.LinearizedStart,
            SizeX = _xAxis.LinearizedEnd - _xAxis.LinearizedStart,
            Y = _yAxis.LinearizedStart,
            SizeY = _yAxis.LinearizedEnd - _yAxis.LinearizedStart,
            Z = _zAxis.LinearizedStart,
            SizeZ = _zAxis.LinearizedEnd - _zAxis.LinearizedStart
        };
    }

    private static Interval GetCommonInterval(IAxisModel axisModel, double lowerBoundOfTraces, double upperBoundOfTraces)
    {
        Interval result;
        if (axisModel != null)
        {
            if (HasValidUserDefinedValue(axisModel.StartExpression))
            {
                result.Lower = axisModel.Start;
            }
            else if (double.IsNaN(lowerBoundOfTraces) || double.IsInfinity(lowerBoundOfTraces))
            {
                result.Lower = axisModel.StartDefault;
            }
            else
            {
                result.Lower = lowerBoundOfTraces;
            }
            if (HasValidUserDefinedValue(axisModel.EndExpression))
            {
                result.Upper = axisModel.End;
            }
            else if (double.IsNaN(upperBoundOfTraces) || double.IsInfinity(upperBoundOfTraces))
            {
                result.Upper = axisModel.EndDefault;
            }
            else
            {
                result.Upper = upperBoundOfTraces;
            }
        }
        else
        {
            result.Lower = lowerBoundOfTraces;
            result.Upper = upperBoundOfTraces;
        }
        if (result.Upper <= result.Lower)
        {
            result.Upper = result.Lower;
        }
        return result;
    }

    private static bool HasValidUserDefinedValue(IAxisLimitEditor axisLimitEditor)
    {
        if (axisLimitEditor != null && !axisLimitEditor.IsDefaultValue)
        {
            return axisLimitEditor.IsValueValid;
        }
        return false;
    }

    private void ComputeTotalTracesCalculationStatus()
    {
        IsCalculating = _traces.Any(trace => trace.CalculatingStatus == CalculatingStatus.Calculating);
    }

    private void FireTraceInserted(IPlotTraceModel3D trace, int index)
    {
        if (TraceInserted != null)
        {
            TraceInserted(trace, index);
        }
    }

    private void FireTraceRemoved(IPlotTraceModel3D trace)
    {
        if (TraceRemoved != null)
        {
            TraceRemoved(trace);
        }
    }

    public void Dispose() { }

    public override bool SetDataObject(global::Quire.Serialization.IDeS11NProvider de11NProvider, global::Quire.PersistentDataObjects.plotType dataObject, global::Quire.Calculation.Core.IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override global::Quire.PersistentDataObjects.plotType GetDataObject(global::Quire.Serialization.IS11NProvider s11NProvider, global::Quire.Calculation.Core.IResultManager resultManager, bool hideFromCalculation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal virtual IPlotEquationModel3D CreateEquationModel()
    {
    return new PlotEquationModel3D();
    }

    internal virtual IPlotTraceModel3D CreateTraceModel(IPlotEquationModel3D plotEquationModel)
    {
    return new PlotTraceModel3D(plotEquationModel)
    {
        Contains = Contains
    };
    }

    public IPlotEquationModel3D AddNewEquation(int index, int previousIndex)
    {
    IPlotEquationModel3D plotEquationModel3D = CreateEquationModel();
    IPlotTraceModel3D trace = CreateTraceModel(plotEquationModel3D);
    plotEquationModel3D.Trace = trace;
    return AddEquation(plotEquationModel3D, index, previousIndex);
    }

    private bool Contains(double xCordinate, double yCoordinate, double zCoordinate)
    {
    if (_xAxis.Contains(xCordinate) && _yAxis.Contains(yCoordinate))
    {
        return _zAxis.Contains(zCoordinate);
    }
    return false;
    }

    public IPlotEquationModel3D AddEquation(IPlotEquationModel3D equationModel, int index, int previousIndex)
    {
    if (DebugTools.AssertNotNull(equationModel, null).Failed)
    {
        return null;
    }
    CheckAndCorrectIndicesForAddOperation(ref index, ref previousIndex, _equationListModel3D);
    _equationListModel3D.Insert(index, equationModel);
    FireEquationAdded(index, previousIndex, equationModel);
    InsertTrace(index, equationModel.Trace);
    return equationModel;
    }

    private void InsertTrace(int index, IPlotTraceModel3D trace)
    {
    trace.PropertyChanged += TracePropertyChanged;
    trace.Changed += OnChanged;
    trace.PlotCalculatableError += TracePlotCalculatableError;
    _traces.Insert(index, trace);
    trace.SetUserDefinedLimits(_xStart, _xEnd, _yStart, _yEnd, _zStart, _zEnd, out var _);
    ComputeTotalTracesSynchronizationStatus();
    FireTraceInserted(trace, index);
    }

    private void FireEquationAdded(int index, int previousindex, IPlotEquationModel3D plotEquationModel)
    {
    if (EquationAdded != null)
    {
        EquationAdded(index, previousindex, plotEquationModel);
    }
    }

    public IPlotEquationModel3D RemoveEquation(IPlotEquation equation, int deletedItemIndex, int previousItemIndex, Direction direction)
    {
    if (DebugTools.DevAssert(ConditionsAreValidForDeleteOperation(deletedItemIndex, previousItemIndex, _equationListModel3D), "Equation list should be non empty").Failed)
    {
        return null;
    }
    CheckAndCorrectDirection(ref direction);
    IPlotEquationModel3D plotEquationModel3D = _equationListModel3D[deletedItemIndex];
    DebugTools.DevAssert(plotEquationModel3D.EquationProxy == equation, null);
    _equationListModel3D.RemoveAt(deletedItemIndex);
    RemoveTrace(plotEquationModel3D.Trace);
    FireEquationRemoved(deletedItemIndex, previousItemIndex, direction);
    return plotEquationModel3D;
    }

    private void FireEquationRemoved(int deletedItemIndex, int priviousItemIndex, Direction direction)
    {
    if (EquationRemoved != null)
    {
        EquationRemoved(deletedItemIndex, priviousItemIndex, direction);
    }
    }

    private static void CheckAndCorrectDirection(ref Direction direction)
    {
    if (DebugTools.DevAssert(direction == Direction.Left || direction == Direction.Right || direction == Direction.Indeterminate, "Direction should be valid").Failed)
    {
        direction = Direction.Left;
    }
    }

    private static bool ConditionsAreValidForDeleteOperation(int index, int previousIndex, IEquationListModel3D list)
    {
    bool isEmpty = list.Count == 0;
    bool previousIndexInRange = 0 <= previousIndex && previousIndex < list.Count;
    bool indexInRange = ((previousIndex <= index && index < list.Count) & previousIndexInRange) && Math.Abs(index - previousIndex) <= 1;
    return !isEmpty & indexInRange;
    }

    private static bool IndicesAreValidForAddOperation(int index, int previousIndex, IEquationListModel3D list)
    {
    bool isEmpty = list.Count == 0;
    bool isStart = index == 0 && previousIndex == 0;
    bool previousIndexInRange = 0 <= previousIndex && previousIndex < list.Count;
    bool indexInRange = ((previousIndex <= index && index <= list.Count) & previousIndexInRange) && Math.Abs(index - previousIndex) <= 1;
    if (!(isEmpty & isStart))
    {
        return !isEmpty & indexInRange;
    }
    return true;
    }

    private void CheckAndCorrectIndicesForAddOperation(ref int index, ref int previousIndex, IEquationListModel3D list)
    {
    if (DebugTools.DevAssert(IndicesAreValidForAddOperation(index, previousIndex, _equationListModel3D), "Equation index Value should be valid").Failed)
    {
        index = list.Count;
        previousIndex = Math.Max(list.Count - 1, 0);
    }
    }

    public PlotModel3D() { }
}
