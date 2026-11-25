using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Controls.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Undo;

namespace Quire.Controls.Plots.Presenter;

internal partial class PlotPresenter
{
    private static void BoundToAxisViewEvents(IPlotAxisView axisView, Action<int, int> addedEventHandler, Action<IPlotEquation, int, int, Direction> removedEventHandler, Action<IPlotEquation> changedEventHandler)
    {
        axisView.RequestAddEquation += addedEventHandler;
        axisView.EquationRemoved += removedEventHandler;
        axisView.ActiveEquationChanged += changedEventHandler;
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

    private void PrimaryYAxis_EquationRemoved(IPlotEquation equation, int deletedItemIndex, int previousItemIndex, Direction direction)
    {
        CommandBuffer.Execute(new RemoveItemCommand(this, _model.PrimaryResultAxis, _model.rangeAxis, _model.PlotLegendModel, deletedItemIndex, equation, previousItemIndex, direction));
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

    private ITraceModel AddTrace(IAxisModel axis1, IEquationItem equation1, IAxisModel axis2, IEquationItem equation2)
    {
        TraceType defaultTraceType = _model.GetDefaultTraceType();
        ITraceModel traceModel = _model.AddTrace(axis1, equation1, axis2, equation2);
        _traceStyleSelector.ApplyNextStyle(traceModel);
        traceModel.Type = defaultTraceType;
        return traceModel;
    }

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

    private static void SetCommonRange(IEnumerable<ITraceModel> traces, IAxisModel axis, Func<ITraceModel, double?> minSelector, Func<ITraceModel, double?> maxSelector)
    {
        double? start = traces.Min(minSelector);
        double? end = traces.Max(maxSelector);
        axis.SetRange(start, end);
    }

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
}
