using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Undo;

namespace Quire.Controls.Plots.Presenter;

internal partial class PlotPresenter
{
    private readonly global::Quire.Controls.Plots.Presenter.IPlotView _view;
    private readonly global::Quire.Controls.Plots.Presenter.IPlotModel _model;
    private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Plots.Presenter.IAxisModel, global::Quire.Controls.Plots.Presenter.IPlotAxisView> _axesMap;
    private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Plots.Presenter.ITraceModel, global::Quire.Controls.Plots.Presenter.IPlotTrace> _tracesMap;
    private readonly global::Quire.Controls.Plots.Presenter.TraceStyleSelector _traceStyleSelector;

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("constructor body follows the field layout; not yet mirrored")]
    public PlotPresenter(global::Quire.Controls.Plots.Presenter.IPlotView view, global::Quire.Controls.Plots.Presenter.IPlotModel model) { }

    private void BoundToEvents()
    {
        BoundToAxisViewEvents(_view.RangeAxis, xAxis_EquationAdded, xAxis_EquationRemoved, xAxis_EquationContolChange);
        new AxisPresenter(_view.RangeAxis, _model.rangeAxis);
        BoundToAxisModelEvents(_model.rangeAxis);
        BoundToAxisViewEvents(_view.PrimaryResultAxis, PrimaryYAxis_EquationAdded, PrimaryYAxis_EquationRemoved, PrimaryYAxis_EquationContolChange);
        new AxisPresenter(_view.PrimaryResultAxis, _model.PrimaryResultAxis);
        BoundToAxisModelEvents(_model.PrimaryResultAxis);
        _view.HasOriginPositioningChanged += View_HasOriginPositioningChanged;
        _view.XLegendPositionsIndexChanged += View_XLegendPositionsIndexChanged;
        _view.YLegendPositionsIndexChanged += View_YLegendPositionsIndexChanged;
        _view.PlotTitlePositionsIndexChanged += View_TitlePositionsIndexChanged;
        _view.PlotLegendPositionsIndexChanged += View_PlotLegendPositionsIndexChanged;
        _view.BackgroundTypePropertyChanged += View_BackgroundTypePropertyChanged;
        if (_view.PlotLegend != null)
        {
            _view.PlotLegend.LegendWidthChanged += View_PlotLegendWidthChanged;
        }
        _model.DeserializationFinished += Model_DeserializationFinished;
        _model.DeserializationStarted += Model_DeserializationStarted;
    }

    private static void BoundToAxisViewEvents(IPlotAxisView axisView, Action<int, int> addedEventHandler, Action<IPlotEquation, int, int, Direction> removedEventHandler, Action<IPlotEquation> changedEventHandler)
    {
        axisView.RequestAddEquation += addedEventHandler;
        axisView.EquationRemoved += removedEventHandler;
        axisView.ActiveEquationChanged += changedEventHandler;
    }

    private void BoundToAxisModelEvents(IAxisModel axisModel)
    {
        axisModel.StartExpression.ValueChanged += () =>
        {
            AxisModelLimitsValueChanged(axisModel, isStart: true, isEnd: false);
        };
        axisModel.SecondTickExpression.ValueChanged += () =>
        {
            AxisModelLimitsValueChanged(axisModel, isStart: false, isEnd: false);
        };
        axisModel.EndExpression.ValueChanged += () =>
        {
            AxisModelLimitsValueChanged(axisModel, isStart: false, isEnd: true);
        };
    }

    private void View_YLegendPositionsIndexChanged(int newPositionIndex)
    {
        _model.PrimaryResultAxis.LegendPositionIndex = newPositionIndex;
    }

    private void View_XLegendPositionsIndexChanged(int newPositionIndex)
    {
        _model.rangeAxis.LegendPositionIndex = newPositionIndex;
    }

    private void View_TitlePositionsIndexChanged(LegendPositions.PlotTitlePosition newPosition)
    {
        _model.PlotTitleModel.Position = newPosition;
    }

    private void View_PlotLegendPositionsIndexChanged(int newPositionIndex)
    {
        _model.PlotLegendModel.Position = (LegendPositions.PlotLegendPosition)newPositionIndex;
    }

    private void View_PlotLegendWidthChanged(double width)
    {
        _model.PlotLegendModel.LegendWidth = width;
    }

    private void View_BackgroundTypePropertyChanged(BackgroundTypeE backgroundType)
    {
        _model.BackgroundType = backgroundType;
    }

    private void Model_DeserializationStarted()
    {
        _view.HasOriginPositioningChanged -= View_HasOriginPositioningChanged;
        _view.UpdateAxisOriginPositions(_model.HasOriginPositioning);
        _view.HasOriginPositioningChanged += View_HasOriginPositioningChanged;
        _view.PlotBackgroundType = _model.BackgroundType;
    }

    private void Model_DeserializationFinished()
    {
        _view.XLegendPositionsIndexChanged -= View_XLegendPositionsIndexChanged;
        _view.YLegendPositionsIndexChanged -= View_YLegendPositionsIndexChanged;
        _view.PlotTitlePositionsIndexChanged -= View_TitlePositionsIndexChanged;
        _view.PlotLegendPositionsIndexChanged -= View_PlotLegendPositionsIndexChanged;
        _view.XLegendPositionIndex = _model.rangeAxis.LegendPositionIndex;
        _view.YLegendPositionIndex = _model.PrimaryResultAxis.LegendPositionIndex;
        _view.PlotTitlePositionIndex = (int)_model.PlotTitleModel.Position;
        _view.PlotLegendPositionIndex = (int)_model.PlotLegendModel.Position;
        if (!double.IsNaN(_model.PlotLegendModel.LegendWidth))
        {
            _view.PlotLegend.LegendWidth = _model.PlotLegendModel.LegendWidth;
        }
        _view.XLegendPositionsIndexChanged += View_XLegendPositionsIndexChanged;
        _view.YLegendPositionsIndexChanged += View_YLegendPositionsIndexChanged;
        _view.PlotTitlePositionsIndexChanged += View_TitlePositionsIndexChanged;
        _view.PlotLegendPositionsIndexChanged += View_PlotLegendPositionsIndexChanged;
    }

    private void View_HasOriginPositioningChanged(bool hasOriginPositioningValue)
    {
        _model.HasOriginPositioning = hasOriginPositioningValue;
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Presenter.PlotPresenter+AddItemCommand, Ptc.Controls.Plots")]
    private partial class AddItemCommand : global::Quire.Undo.IUndoableCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.PlotPresenter _presenter;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _axis;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _otherAxis;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IPlotLegendModel _plotLegend;
        [global::Quire.Mirror.MirrorStub]
        private readonly int _index;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Plots.Presenter.ITraceModel _trace;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Plots.Presenter.IEquationListItem _item;
        [global::Quire.Mirror.MirrorStub]
        private readonly int _previousIndex;
        [global::Quire.Mirror.MirrorStub]
        public string Name { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
        [global::Quire.Mirror.MirrorStub]
        public AddItemCommand(global::Quire.Controls.Plots.Presenter.PlotPresenter presenter, global::Quire.Controls.Plots.Presenter.IAxisModel axis, global::Quire.Controls.Plots.Presenter.IAxisModel otherAxis, global::Quire.Controls.Plots.Presenter.IPlotLegendModel plotLegend, int index, int previousIndex) { }
        [global::Quire.Mirror.MirrorStub]
        public bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Presenter.PlotPresenter+RemoveItemCommand, Ptc.Controls.Plots")]
    private partial class RemoveItemCommand : global::Quire.Undo.IUndoableCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.PlotPresenter _presenter;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _axis;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IAxisModel _otherAxis;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IPlotLegendModel _plotLegend;
        [global::Quire.Mirror.MirrorStub]
        private readonly int _index;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Plots.Presenter.ITraceModel _trace;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Plots.Presenter.IPlotEquation _equation;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Plots.Presenter.IEquationListItem _item;
        [global::Quire.Mirror.MirrorStub]
        private readonly int _previousItemIndex;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Direction _direction;
        [global::Quire.Mirror.MirrorStub]
        public string Name { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
        [global::Quire.Mirror.MirrorStub]
        public RemoveItemCommand(global::Quire.Controls.Plots.Presenter.PlotPresenter presenter, global::Quire.Controls.Plots.Presenter.IAxisModel axis, global::Quire.Controls.Plots.Presenter.IAxisModel otherAxis, global::Quire.Controls.Plots.Presenter.IPlotLegendModel plotLegend, int index, global::Quire.Controls.Plots.Presenter.IPlotEquation equation, int priviousItemIndex, global::Quire.Direction direction) { }
        [global::Quire.Mirror.MirrorStub]
        public bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    private void PrimaryYAxis_EquationAdded(int index, int previousIndex)
    {
        CommandBuffer.Execute(new AddItemCommand(this, _model.PrimaryResultAxis, _model.rangeAxis, _model.PlotLegendModel, index, previousIndex));
        if (_view.ActivePlotEquation == null)
        {
            _view.ActivePlotEquation = GetViewAxis(_model.PrimaryResultAxis)[0];
        }
    }

    private void xAxis_EquationAdded(int index, int previousIndex)
    {
        CommandBuffer.Execute(new AddItemCommand(this, _model.rangeAxis, _model.PrimaryResultAxis, _model.PlotLegendModel, index, previousIndex));
    }

    private void xAxis_EquationRemoved(IPlotEquation equation, int deletedItemIndex, int priviousItemIndex, Direction direction)
    {
        CommandBuffer.Execute(new RemoveItemCommand(this, _model.rangeAxis, _model.PrimaryResultAxis, _model.PlotLegendModel, deletedItemIndex, equation, priviousItemIndex, direction));
    }

    private void xAxis_EquationContolChange(IPlotEquation equation)
    {
        IEquationItem equationItem = _model.rangeAxis.Equations.FirstOrDefault((IEquationItem item) => item.Equation == equation);
        if (equationItem != null)
        {
            HandleSelectTraces(equationItem.Traces);
        }
    }

    private void PrimaryYAxis_EquationContolChange(IPlotEquation equation)
    {
        IEquationItem equationItem = _model.PrimaryResultAxis.Equations.FirstOrDefault((IEquationItem item) => item.Equation == equation);
        if (equationItem != null)
        {
            HandleSelectTraces(equationItem.Traces);
        }
    }

    private void PrimaryYAxis_EquationRemoved(IPlotEquation equation, int deletedItemIndex, int previousItemIndex, Direction direction)
    {
        CommandBuffer.Execute(new RemoveItemCommand(this, _model.PrimaryResultAxis, _model.rangeAxis, _model.PlotLegendModel, deletedItemIndex, equation, previousItemIndex, direction));
    }

    private global::Quire.Controls.Plots.Presenter.ITraceModel HandleEquationAdded(global::Quire.Controls.Plots.Presenter.IAxisModel axis1, global::Quire.Controls.Plots.Presenter.IAxisModel axis2, global::Quire.Controls.Plots.Presenter.IPlotLegendModel plotLegendModel, global::Quire.Controls.Plots.Presenter.ITraceModel trace, int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private ITraceModel AddTrace(IAxisModel axis1, IEquationItem equation1, IAxisModel axis2, IEquationItem equation2)
    {
        TraceType defaultTraceType = _model.GetDefaultTraceType();
        ITraceModel traceModel = _model.AddTrace(axis1, equation1, axis2, equation2);
        _traceStyleSelector.ApplyNextStyle(traceModel);
        traceModel.Type = defaultTraceType;
        return traceModel;
    }

    private void HandleSelectTraces(IEnumerable<ITraceModel> Traces)
    {
        List<IPlotTrace> list = new List<IPlotTrace>();
        foreach (ITraceModel Trace in Traces)
        {
            list.Add(GetViewTrace(Trace));
        }
        _view.SelectedTraces = list;
    }

    private global::Quire.Controls.Plots.Presenter.ITraceModel HandleEquationRemoved(global::Quire.Controls.Plots.Presenter.IPlotLegendModel plotLegendModel, global::Quire.Controls.Plots.Presenter.IEquationItem eq_item, global::Quire.Controls.Plots.Presenter.AxesType eqAxis, global::Quire.Controls.Plots.Presenter.AxesType otherAxis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void RemovePlotLegendItemFromPlotLegend(IPlotLegendModel plotLegendModel, ITraceModel trace)
    {
        IPlotLegendItem legendItem = _view.PlotLegend.RemoveTraceFromPlotLegend(GetViewTrace(trace));
        plotLegendModel.RemoveLegendItem(legendItem);
    }

    private void AddTraceToEquation(ITraceModel trace, IEquationItem equationItem, IAxisModel axis)
    {
        equationItem.AddTrace(trace);
        GetViewAxis(axis).AddTraceToEquation(GetViewTrace(trace), equationItem.Equation);
    }

    private void AddTraceToEquation(ITraceModel trace, IEquationItem equationItem, IAxisModel axis, int index)
    {
        equationItem.AddTrace(trace, index);
        GetViewAxis(axis).AddTraceToEquation(GetViewTrace(trace), equationItem.Equation, index);
    }

    private void RemoveTraceFromEquation(int traceIndex, IEquationItem equationItem, IAxisModel axis)
    {
        equationItem.RemoveTrace(traceIndex);
        GetViewAxis(axis).RemoveTraceFromEquation(traceIndex, equationItem.Equation);
    }

    private void RearrangeTraces(IAxisModel axis)
    {
        int num = 0;
        IEnumerable<IEquationItem> equations = axis.Equations;
        foreach (IEquationItem item in equations)
        {
            num++;
            if (equations.Count() <= num)
            {
                break;
            }
            IEquationItem equationItem = equations.ElementAt(num);
            switch (item.Traces.Count())
            {
            case 0:
                if (equationItem != null && equationItem.Traces.Count() != 0)
                {
                    MoveFirstTrace(equationItem, item, axis);
                }
                break;
            default:
                if (equationItem != null)
                {
                    MoveAllButFirstTrace(item, equationItem, axis);
                }
                break;
            case 1:
                break;
            }
        }
    }

    private void MoveFirstTrace(IEquationItem source, IEquationItem target, IAxisModel axis)
    {
        ITraceModel traceModel = source.Traces.First();
        if (traceModel != null)
        {
            traceModel.SetEquation(AxesTypeToIndex.Index(axis.Type), target);
            GetViewTrace(traceModel).SetEquation(axis.Type, target.Equation);
            RemoveTraceFromEquation(0, source, axis);
            AddTraceToEquation(traceModel, target, axis);
        }
    }

    private void MoveAllButFirstTrace(IEquationItem source, IEquationItem target, IAxisModel axis)
    {
        List<ITraceModel> list = source.Traces.Skip(1).ToList();
        int num = 0;
        // Enumerating through IEnumerable<T> is what Mathcad's IL calls; a List<T> loop differs.
        foreach (ITraceModel item in (IEnumerable<ITraceModel>)list)
        {
            item.SetEquation(AxesTypeToIndex.Index(axis.Type), target);
            GetViewTrace(item).SetEquation(axis.Type, target.Equation);
            RemoveTraceFromEquation(1, source, axis);
            AddTraceToEquation(item, target, axis, num++);
        }
    }

    private IPlotAxisView GetViewAxis(IAxisModel axisModel)
    {
        return _axesMap[axisModel];
    }

    private IPlotTrace GetViewTrace(ITraceModel traceModel)
    {
        return _tracesMap[traceModel];
    }

    private void SetCommonRange(ITraceModel traceModel)
    {
        IAxisModel xAxis = traceModel.GetAxis(AxesIndex.Range);
        IAxisModel yAxis = traceModel.GetAxis(AxesIndex.Result);
        IEnumerable<ITraceModel> traces = _model.Traces.Where((ITraceModel trace) => xAxis == trace.GetAxis(AxesIndex.Range) || yAxis == trace.GetAxis(AxesIndex.Result));
        SetRangeAxisCommonRange(traces, xAxis);
        SetResultAxisCommonRange(traces, yAxis);
    }

    private void AxisModelLimitsValueChanged(IAxisModel axisModel, bool isStart, bool isEnd)
    {
        DebugTools.DevAssert(!isStart || !isEnd, null);
        if (isStart)
        {
            axisModel.StartValueChanged();
        }
        AxesType type = axisModel.Type;
        AxesIndex axesIndex = AxesTypeToIndex.Index(type);
        IEnumerable<ITraceModel> enumerable = _model.Traces.Where((ITraceModel trace) => trace.GetAxis(axesIndex) == axisModel);
        switch (axesIndex)
        {
            case AxesIndex.Range:
                if (isStart | isEnd)
                {
                    enumerable.Each((ITraceModel trace) =>
                    {
                        trace.RangeAxisLimitValueChanged();
                    });
                }
                SetRangeAxisCommonRange(enumerable, axisModel);
                break;
            case AxesIndex.Result:
                if (isStart | isEnd)
                {
                    enumerable.Each((ITraceModel trace) =>
                    {
                        trace.ResultAxisLimitValueChanged();
                    });
                }
                SetResultAxisCommonRange(enumerable, axisModel);
                break;
        }
    }

    private static void SetRangeAxisCommonRange(IEnumerable<ITraceModel> traces, IAxisModel xAxis)
    {
        Func<ITraceModel, double?> minSelector;
        if (xAxis.Scaling == AxisScaling.Linear)
        {
            minSelector = (ITraceModel trace) =>
            {
                // The IL re-reads TopLeft after the NaN check.
                Point topLeft = trace.TopLeft;
                if (!double.IsNaN(topLeft.X))
                {
                    topLeft = trace.TopLeft;
                    return topLeft.X;
                }
                return (double?)null;
            };
        }
        else
        {
            minSelector = (ITraceModel trace) => trace.MinPositiveX;
        }
        SetCommonRange(traces, xAxis, minSelector, (ITraceModel trace) =>
        {
            Point bottomRight = trace.BottomRight;
            if (!double.IsNaN(bottomRight.X))
            {
                bottomRight = trace.BottomRight;
                return bottomRight.X;
            }
            return (double?)null;
        });
    }

    private static void SetResultAxisCommonRange(IEnumerable<ITraceModel> traces, IAxisModel yAxis)
    {
        Func<ITraceModel, double?> minSelector;
        if (yAxis.Scaling == AxisScaling.Linear)
        {
            minSelector = (ITraceModel trace) =>
            {
                // The IL re-reads TopLeft after the NaN check.
                Point topLeft = trace.TopLeft;
                if (!double.IsNaN(topLeft.Y))
                {
                    topLeft = trace.TopLeft;
                    return topLeft.Y;
                }
                return (double?)null;
            };
        }
        else
        {
            minSelector = (ITraceModel trace) => trace.MinPositiveY;
        }
        SetCommonRange(traces, yAxis, minSelector, (ITraceModel trace) =>
        {
            Point bottomRight = trace.BottomRight;
            if (!double.IsNaN(bottomRight.Y))
            {
                bottomRight = trace.BottomRight;
                return bottomRight.Y;
            }
            return (double?)null;
        });
    }

    private static void SetCommonRange(IEnumerable<ITraceModel> traces, IAxisModel axis, Func<ITraceModel, double?> minSelector, Func<ITraceModel, double?> maxSelector)
    {
        double? start = traces.Min(minSelector);
        double? end = traces.Max(maxSelector);
        axis.SetRange(start, end);
    }

    private void CheckTraceVsPlotType(global::Quire.Controls.Plots.Presenter.ITraceModel traceModel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}

